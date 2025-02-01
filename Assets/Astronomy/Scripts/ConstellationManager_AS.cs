using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class ConstellationManager_AS : MonoBehaviour
        {
            public enum ConstellationName
            {
                Cthulhu,
                Snake,
                Cake,
                Sofa
            }
            [Header("constellations")]
            [SerializeField] private ConstellationName[] normalConstellationArr;
            [SerializeField] private float chooseNormalChance;
            [SerializeField] private ConstellationName[] rareConstellationArr;
            [SerializeField] private float constellationShowTime;
            private StarFormation_AS _chosenFormation;
            
            [Header("FromFormation")]
            private SpriteRenderer _sprRend;
            private SpriteRenderer _artBG;//blackBG
            private Transform _starHolder;
            private List<StarBehavior_AS> _starList = new List<StarBehavior_AS>();

            [SerializeField] private float stretchMinAmount;
            [SerializeField] private float stretchMaxAmount;
            [SerializeField] private float blackBGTransparency;

            [Header("CelebrationSweep")]
            [SerializeField] private float sweepSpeed;
            private float _sweepPos;

            public void ChooseConstellation(Background_AS bg)
            {
                /*if (DebugMode)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        _chosenConstellation = normalConstellationArr[0].GetComponent<Constellation_AS>();
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        _chosenConstellation = Instantiate(normalConstellationArr[1], constellationHolder).GetComponent<Constellation_AS>();
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        _chosenConstellation = Instantiate(normalConstellationArr[2], constellationHolder).GetComponent<Constellation_AS>();
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        _chosenConstellation = Instantiate(normalConstellationArr[3], constellationHolder).GetComponent<Constellation_AS>();
                    }
                    return;
                }*/
                if (Random.Range(0f, 1f) < chooseNormalChance)
                {
                    ConstellationName _constellationName = normalConstellationArr[Random.Range(0, normalConstellationArr.Length)];
                    _chosenFormation = bg.FindStarFormation(_constellationName).gameObject.GetComponent<StarFormation_AS>();
                }
                else
                {
                    ConstellationName _constellationName = rareConstellationArr[Random.Range(0, rareConstellationArr.Length)];
                    _chosenFormation = bg.FindStarFormation(_constellationName).gameObject.GetComponent<StarFormation_AS>();
                }
                OnConstellationChosen(bg);
            }
            public void OnConstellationChosen(Background_AS bg)
            {
                _starHolder = _chosenFormation.StarHolder;
                _sprRend = _chosenFormation.ArtSprRend;
                _artBG = bg.ArtBGRend;
                _sprRend.color = new Color(_sprRend.color.r, _sprRend.color.g, _sprRend.color.b, 0);
                foreach (Transform _child in _starHolder)
                {
                    _starList.Add(_child.gameObject.GetComponent<StarBehavior_AS>());
                }
                foreach (StarBehavior_AS _star in _starList)
                {
                    _star.CompleteNeighbors();
                }
                //Can only create line rend after all neighbors are checked
                foreach (StarBehavior_AS _star in _starList)
                {
                    _star.CreateLineRenderer();
                }
                RandomizePosition();
            }

            void RandomizePosition()
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                float stretchXAmount = Random.Range(stretchMinAmount,stretchMaxAmount);
                float stretchYAmount = Random.Range(stretchMinAmount, stretchMaxAmount);
                foreach (Transform _child in _starHolder)
                {
                    _child.position = new Vector3(_child.position.x * stretchXAmount, _child.position.y * stretchYAmount);
                }
            }
            public void ShowConstellation()
            {
                StartCoroutine(DoShowConstellationArt(constellationShowTime));
            }
            public void CheckStarConnection()
            {
                for (int i = 0; i < _starList.Count; i++)
                {
                    if (_starList[i].HasClicked)
                    {
                        _starList[i].ConnectionCheck();
                    }
                }
            }
            IEnumerator DoShowConstellationArt(float _fadeTime)
            {
                float totalTime = 0;
                while (totalTime < _fadeTime)
                {
                    totalTime += Time.deltaTime;
                    _sprRend.color = new Color(_sprRend.color.r, _sprRend.color.g, _sprRend.color.b, totalTime / _fadeTime);
                    _artBG.color = new Color(0, 0, 0, (totalTime / _fadeTime)*blackBGTransparency);
                    yield return new WaitForEndOfFrame();
                }
                _sprRend.color = Color.white;
            }
            public bool WinCheck()
            {
                foreach (StarBehavior_AS _star in _starList)
                {
                    if(!_star.HasClicked)
                    {
                        return false;
                    }
                }
                return true;
            }
            public void CelebrationSweep(float startXPos, float endXPos)
            {
                _sweepPos = startXPos;
                StartCoroutine(DoCelebrationSweep(startXPos,endXPos));
            }
            IEnumerator DoCelebrationSweep(float startXPos, float endXPos)
            {
                while (_sweepPos < endXPos)
                {
                    _sweepPos+= Time.deltaTime * sweepSpeed;
                    foreach(StarBehavior_AS _star in _starList)
                    {
                        if (!_star.IsCelebrateRotation && _star.transform.position.x < _sweepPos)
                        {
                            _star.CelebrateRotate();
                        }
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
            public List<StarBehavior_AS> GetStarList()
            {
                return _starList;
            }
            public void CleanUpList()
            {
                _starList.Clear();
                StopAllCoroutines();
            }
            private void OnDisable()
            {
                StopAllCoroutines();
            }
        }
    }
}