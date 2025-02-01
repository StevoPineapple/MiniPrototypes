using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace RitualNight
{
    namespace PartyGames
    {
        public class AstronomyTaskBehavior : MonoBehaviour//TaskBehavior
        {
            public bool HasWon;
            public bool DebugMode;
            [SerializeField] private float showConstellationDelay;

            [Header("Component")]
            [SerializeField] private ConstellationManager_AS constellationManager;
            [SerializeField] private PlayerController_AS playerController;

            [Header("BG")]
            [SerializeField] private Background_AS[] normalBGArr;
            [SerializeField] private Transform bgHolder;
            private Background_AS _chosenBackground;

            [Header("photoBG")]
            public SpriteRenderer photoBGSprRend;
            [SerializeField] private SpriteRenderer bigPhotoRef;
            [SerializeField] private SpriteRenderer smallPhotoRef;
            [SerializeField] private Transform photoStarParent;
            [SerializeField] private GameObject photoStarPrefab;
            private List<GameObject> _photoStarList = new List<GameObject>();
            [SerializeField] private GameObject photoLinePrefab;

            [Header("Stars")]
            [SerializeField] private GameObject starPrefab;
            private float _starColliderDiameter;
            [SerializeField] private PolygonCollider2D spawnRegion;
            [SerializeField] private int maxBadStar;
            [SerializeField] private int minBadStar;
            [SerializeField] private Transform badStarHolder;

            [Header("EasyMode")]
            [SerializeField] private PartyGameEasyModeTimer easyModeTimer;
            public delegate void OnStartEasyMode();
            public static event OnStartEasyMode onEasyModeEvt;

            [Header ("MiniManager")]
            public AstronomyMiniManager ASMM;

            public void Open() //override 
            {
                //PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                easyModeTimer.StartTimer();
                CleanUpList();
                constellationManager.CleanUpList();
                _starColliderDiameter = starPrefab.GetComponent<CircleCollider2D>().radius * 2;
                ChooseBG();
                constellationManager.ChooseConstellation(_chosenBackground);
                CreatePhotoStars();
                StartCoroutine(DoDelayGenerateStars());
                HasWon = false;
                playerController.StartGame();
                //base.StartOpen();
            }
            //Or else the overlappoint won't work with changed scale
            private IEnumerator DoDelayGenerateStars()
            {
                yield return new WaitForFixedUpdate();
                GenerateBadStars();
            }
            private IEnumerator DoFinishTask()
            {
                //SetPartyGameResult(true);
                yield return new WaitForSeconds(0.7f);
                ASMM.SetWin();
                //SetStateClosing();
            }
                            
            public void StartClose() //override 
            {
                onEasyModeEvt = null;
                CleanUpList();
                constellationManager.CleanUpList();
                StopAllCoroutines();
                //PlayerController.CloseGame();
                //PlayerController.gameObject.SetActive(false);

                //base.StartClose();
            }
            private void GenerateBadStars()
            {
                int starCount = Random.Range(minBadStar, maxBadStar);
                for (int i = 0; i < starCount; i++)
                {
                    int _breakTick = 500;
                    GameObject _currStar = Instantiate(starPrefab,badStarHolder);
                    do
                    {
                        Vector2 _spawnPos = new Vector2(
                            Random.Range(spawnRegion.bounds.min.x, spawnRegion.bounds.max.x),
                            Random.Range(spawnRegion.bounds.min.y, spawnRegion.bounds.max.y));

                        _currStar.transform.position = _spawnPos;
                        _breakTick--;
                        if (_breakTick < 0)
                        {
                            Debug.LogError("whileLoop over 500 in AS star generation");
                            break;
                        }
                    }
                    while (!CheckStarPositionIsOK(_currStar));
                }
            }
            public bool CheckPosition(Vector3 pos)
            {
                if (!spawnRegion.OverlapPoint(spawnRegion.transform.InverseTransformPoint(pos)))
                {
                    return false;
                }
                //check if overlapping with BG
                if (_chosenBackground.BGCollider.OverlapPoint(_chosenBackground.BGCollider.transform.InverseTransformPoint(pos)))
                {
                    return false;
                }
                return true;
            }
            public bool CheckWithObj(GameObject obj)
            {
                return CheckStarPositionIsOK(obj);
            }
            public int gizmoAmount;
            public bool inverse;
            public float rad;
            private void OnDrawGizmos()
            {
                if (!DebugMode)
                {
                    return;
                }
                for(int i = 0; i < gizmoAmount; i++) 
                {
                    if (inverse)
                    {
                       Vector3 pos = new Vector3(Random.Range(spawnRegion.bounds.min.x, spawnRegion.bounds.max.x),
                            Random.Range(spawnRegion.bounds.min.y, spawnRegion.bounds.max.y));
                        if (!spawnRegion.OverlapPoint(pos)||
                            _chosenBackground.BGCollider.OverlapPoint(pos))
                        { Gizmos.color = Color.red; } else { Gizmos.color = Color.green; }
                       Gizmos.DrawSphere(pos, rad);
                    }
                    else
                    {
                        Vector3 pos = new Vector3(Random.Range(spawnRegion.bounds.min.x, spawnRegion.bounds.max.x),
                            Random.Range(spawnRegion.bounds.min.y, spawnRegion.bounds.max.y));
                        if (!spawnRegion.OverlapPoint(spawnRegion.transform.TransformPoint(pos)) &&
                            _chosenBackground.BGCollider.OverlapPoint(_chosenBackground.BGCollider.transform.TransformPoint(pos)))
                        { Gizmos.color = Color.red; }
                        else { Gizmos.color = Color.green; }
                        Gizmos.DrawSphere(pos, rad);
                    }
                }
            }
            private bool CheckStarPositionIsOK(GameObject _currStar)
            {
                Vector3 _starPos = _currStar.transform.position;
                //check if is in spawn region
                if (!spawnRegion.OverlapPoint(_starPos)|| _chosenBackground.BGCollider.OverlapPoint(_starPos))
                {
                    return false;
                }
                //check if overlapping with BG
                if (_chosenBackground.BGCollider.OverlapPoint(_starPos))
                {
                    return false;
                }
                //overlapping with otherstars?
                foreach (Transform _badstar in badStarHolder)
                {
                    if (_badstar.gameObject != _currStar)
                    {
                        if (Vector3.Distance(_starPos, _badstar.gameObject.transform.position) < _starColliderDiameter)
                        {
                            return false;
                        }
                    }
                }
                //overlapping with Constellation?
                foreach (StarBehavior_AS _goodstar in constellationManager.GetStarList())
                {
                    if (Vector3.Distance(_starPos, _goodstar.gameObject.transform.position) < _starColliderDiameter)
                    {
                        return false;
                    }
                }
                return true;
            }
            private void ChooseBG()
            {
                _chosenBackground = Instantiate(normalBGArr[Random.Range(0, normalBGArr.Length)], bigPhotoRef.bounds.min, Quaternion.identity, bgHolder).GetComponent<Background_AS>();
                float _fitToBigPhotoScale = (bigPhotoRef.bounds.max.x - bigPhotoRef.bounds.min.x) / (_chosenBackground.BGSprRend.bounds.max.x - _chosenBackground.BGSprRend.bounds.min.x);
                _chosenBackground.gameObject.transform.localScale = new Vector3(_fitToBigPhotoScale, _fitToBigPhotoScale, _fitToBigPhotoScale);

                photoBGSprRend.sprite = _chosenBackground.photoBG;
                float _fitToSmallPhotoScale = (smallPhotoRef.bounds.max.x - smallPhotoRef.bounds.min.x) / (photoBGSprRend.bounds.max.x - photoBGSprRend.bounds.min.x);
                photoBGSprRend.transform.position = smallPhotoRef.bounds.min;
                photoBGSprRend.transform.localScale *= _fitToSmallPhotoScale;
            }
            private IEnumerator DoDelay()
            {
                yield return new WaitForFixedUpdate();
                GenerateBadStars();
            }
            private void CreatePhotoStars()
            {
                List<StarBehavior_AS> _starList = constellationManager.GetStarList();
                float _bigToSmallScale = (smallPhotoRef.bounds.max.x - smallPhotoRef.bounds.min.x) / (bigPhotoRef.bounds.max.x - bigPhotoRef.bounds.min.x);

                //Create all the photostars
                foreach (StarBehavior_AS _star in _starList)
                {
                    Vector3 _starPos = GetPositionInPhoto(_star.transform.position, _bigToSmallScale);
                    //smallPhotoRef.bounds.min + ((_star.transform.position - bigPhotoRef.bounds.min) * _bigToSmallScale);
                    GameObject _photoStar = Instantiate(photoStarPrefab, _starPos, Quaternion.identity, photoStarParent);
                    _photoStarList.Add(_photoStar);
                } 

                //Create linerenderer in photo based on linerenderer position of real stars.
                for(int i = 0; i<_starList.Count;i++)
                {
                    List<StarBehavior_AS> _currStarList = _starList[i].GetNeighbors();
                    foreach(StarBehavior_AS _neighbor in _currStarList)
                    {
                        LineRenderer _line = Instantiate(photoLinePrefab, _photoStarList[i].transform.position, Quaternion.identity, photoStarParent).GetComponent<LineRenderer>();
                        _line.SetPosition(0, _photoStarList[i].transform.position);
                        _line.SetPosition(1, Vector3.Lerp(_photoStarList[i].transform.position, GetPositionInPhoto(_neighbor.transform.position, _bigToSmallScale), Random.Range(0.3f,0.7f)));
                        _line.SetPosition(2, GetPositionInPhoto(_neighbor.transform.position, _bigToSmallScale));
                    }
                }
            }
            public void OnGoodStarClick()
            {
                constellationManager.CheckStarConnection();
                if (constellationManager.WinCheck())
                {
                    StartCoroutine(DoWinDelay());
                }
            }
            IEnumerator DoWinDelay()
            {
                playerController.SetCannotInteract();
                yield return new WaitForSeconds(0.2f);
                constellationManager.CelebrationSweep(bigPhotoRef.bounds.min.x, bigPhotoRef.bounds.max.x);
                yield return new WaitForSeconds(showConstellationDelay);
                constellationManager.ShowConstellation();
                WinCheck();
            }
            public void WinCheck()
            {
                HasWon = true;
                StartCoroutine(DoFinishTask());
            }
            public Background_AS GetChosenBG()
            {
                return _chosenBackground;
            }
            private void CleanUpList()
            {
                foreach (Transform child in badStarHolder)
                {
                    Destroy(child.gameObject);
                }
                foreach (Transform child in photoStarParent)
                {
                    Destroy(child.gameObject);
                }
                if (_chosenBackground != null)
                {
                    Destroy(_chosenBackground.gameObject);
                }
                _photoStarList.Clear();
                StopAllCoroutines();
            }
            Vector3 GetPositionInPhoto(Vector3 _realPos, float _bigToSmallScale)
            {
                return smallPhotoRef.bounds.min + ((_realPos - bigPhotoRef.bounds.min) * _bigToSmallScale);
            }
            public void StartEasyMode()
            {
                onEasyModeEvt?.Invoke();
            }
        }
    }
}