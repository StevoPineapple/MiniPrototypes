using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
//using RitualNight.Variables;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PlayerController_DV : MonoBehaviour
        {
            /*[Header("PartyControl")]
            [SerializeField] private BoolReference isMobile;
            [SerializeField] private PartyGameController PartyGameController;
            */

            [Header ("Components")]
            public DVDTaskBehavior DVManager;
            [SerializeField] private TvController_DV tvController;
            [SerializeField] private PolygonCollider2D spawnArea;

            [Header ("Prefab")]
            [SerializeField] private GameObject logoPrefab;
            [SerializeField] private GameObject trajectoryPrefab;

            [Header ("Parents")]
            [SerializeField] private Transform logoParent;
            [SerializeField] private Transform trajectoryObjParent;
            [SerializeField] private Transform dotParent;
            [SerializeField] private Transform ghostParent;

            private LogoBehavior_DV _currentDisc;
            private TrajectoryLogo_DV _trajectoryDisc;
            
            [Header ("States")]
            public bool IsSlow;
            public bool IsMidSlow;
            private Vector3 _lastPos;
            private KnobBehavior_DV _currentKnob;
            public bool IsGrabbing;
            //true in flashing, false only when hit wall 

            [Header ("Para")]
            [SerializeField] private int trajectoryLife;
            [SerializeField] private float trajectorySpeedMulti;
            [SerializeField] private float slowRate;
            private float slowRateReverse;
            [SerializeField] private int slowStickTickMax;
            private int _slowStickTick;

            public bool CanScoreLogo;
            public bool CanGrab;

            [Header ("Debug")]
            public bool DebugMode;

            /*private void OnEnable()
            {
                if (PartyGameController.IsUsingController)
                {
                    PartyGameController.OnPartyGameActionPress += OnControllerPress;
                    PartyGameController.OnPartyGameActionRelease += OnControllerRelease;
                }
                else
                {
                    PartyGameController.OnPartyGameActionPress += OnMousePress;
                    if (isMobile.Value)
                    {
                        PartyGameController.OnPartyGameActionReleaseMobile += OnMouseRelease;
                    }
                    else
                    {
                        PartyGameController.OnPartyGameActionRelease += OnMouseRelease;
                    }
                }
            }

            private void OnDisable()
            {
                if (PartyGameController.IsUsingController)
                {
                    PartyGameController.OnPartyGameActionPress -= OnControllerPress;
                    PartyGameController.OnPartyGameActionRelease -= OnControllerRelease;
                }
                else
                {
                    PartyGameController.OnPartyGameActionPress -= OnMousePress;
                    if (isMobile.Value)
                    {
                        PartyGameController.OnPartyGameActionReleaseMobile -= OnMouseRelease;
                    }
                    else
                    {
                        PartyGameController.OnPartyGameActionRelease -= OnMouseRelease;
                    }
                }
            }*/
            public void StartGame()
            {
                CanGrab = false;
                if (slowRate != 0)
                {
                    slowRateReverse = 1 / slowRate;
                }
                else
                {
                    throw new System.Exception("slowrate is 0. diveded by 0");
                }
                tvController.StartGame();
                CreateNewLogo(0);
            }

            public void IncreseScore(int amount)
            {
                DVManager.IncreaseScore(amount);
            }

            public void CreateNewLogo(int _bounce)
            {
                StartCoroutine(DoCreateNewLogo(_bounce));
            }
            IEnumerator DoCreateNewLogo(int _bounce) //beware of using StopAllCoroutines() because it might stop this
            {
                if (_bounce >= 3)
                {
                    yield return new WaitForSeconds(tvController.WinFlashTotalTime + 0.05f);
                }
                else if(_bounce == 2)
                {
                    yield return new WaitForSeconds(tvController.WinFlashTotalTime + 0.15f);
                }
                else
                {
                    yield return new WaitForSeconds(tvController.WinFlashTotalTime + 0.5f);
                }
                Vector3 _spawnPos = GetRandomPointInPolygon(spawnArea);

                _currentDisc = Instantiate(logoPrefab, _spawnPos, transform.rotation, logoParent).GetComponent<LogoBehavior_DV>();
                _currentDisc.PlayerController = this;
                _currentDisc.SetValues(logoParent, trajectoryObjParent, dotParent, ghostParent, tvController, trajectoryPrefab, trajectoryLife, slowRate, trajectorySpeedMulti);

                _trajectoryDisc = _currentDisc.CreateLaunchCopy(_spawnPos - _currentDisc.transform.position);

                //Random Angle
                float xx = Random.Range(1.8f, 2.2f);
                float yy = Random.Range(1.8f, 2.2f);
                xx = Random.Range(0f, 1f) > 0.5f ? -xx : xx;
                yy = Random.Range(0f, 1f) > 0.5f ? -yy : yy;
                _currentDisc.SetAngle(new Vector3(xx,yy, 0));

                //_isAiming = false;
                _currentDisc.Launch();
                tvController.CurrentLogo = _currentDisc;
                _lastPos = transform.position;
            }

            void DebugCreateLogo() //Usused
            {
                if (!DebugMode)
                {
                    return;
                }
                if (Input.GetMouseButtonDown(1))
                {

                    Vector3 _spawnPos = GetRandomPointInPolygon(spawnArea);

                    _currentDisc = Instantiate(logoPrefab, transform.position, transform.rotation, logoParent).GetComponent<LogoBehavior_DV>();
                    _currentDisc.PlayerController = this;
                    _currentDisc.SetValues(logoParent, trajectoryObjParent, dotParent, ghostParent, tvController, trajectoryPrefab, trajectoryLife, slowRate, trajectorySpeedMulti);

                    _trajectoryDisc = _currentDisc.CreateLaunchCopy(transform.position - _currentDisc.transform.position);

                    //_currentDisc.SetAngle(transform.position-_currentDisc.transform.position);

                    
                    //_currentDisc.Launch();
                    //StartCoroutine(DoLaunchCopies());
                    _lastPos = transform.position;
                }
                if (Input.GetMouseButton(1))
                {
                    _currentDisc.SetAngle(transform.position - _currentDisc.transform.position);
                    _lastPos = transform.position;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    
                    _currentDisc.Launch();
                    //StopAllCoroutines();
                    foreach (Transform child in dotParent)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            private void Update()
            {
                //DebugCreateLogo();
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                //PartyGameController.PartyGameFollow(transform, PartyGameController.IsUsingController ? 5 : 100);
                if (Input.GetMouseButtonDown(0))
                {
                    OnMousePress();
                }
                if (Input.GetMouseButton(0))
                {
                    OnMouseHold();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    OnMouseRelease();
                }
                if (IsSlow)
                {
                    tvController.ChangeSpeedIcon(0);
                }
                else if (IsMidSlow)
                {
                    if (_slowStickTick > 0)
                    {
                        tvController.ChangeSpeedIcon(1);
                        _slowStickTick--;
                    }
                    else
                    {
                        IsMidSlow = false;
                    }
                }
                else
                {
                    tvController.ChangeSpeedIcon(2);
                }
            }
            private void OnTriggerStay2D(Collider2D collision)
            {
                if (IsGrabbing || !CanGrab)
                {
                    return;
                }
                if (collision.CompareTag("Knob_DV"))
                {   
                    KnobBehavior_DV knob = collision.GetComponent<KnobBehavior_DV>();
                    
                    if (knob.IsActive)
                    {
                        _currentKnob = knob;
                        _currentKnob.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    }
                }
            }
            private void OnTriggerExit2D(Collider2D collision)
            {
                if (IsGrabbing || !CanGrab || _currentKnob == null)
                {
                    return;
                }
                if (collision.CompareTag("Knob_DV"))
                {
                    _currentKnob.transform.localScale = new Vector3(1f, 1f, 1f);
                    _currentKnob = null;
                }
            }
            public void ForceRelease(int _releaseSide)
            {
                if (_currentKnob == null)
                {
                    return;
                }

                _currentKnob.transform.localScale = new Vector3(1f, 1f, 1f);

                tvController.ForceReleaseFlash(_releaseSide);
                IsSlow = false;
                _currentKnob.OnRelease();
                _currentKnob = null;
                IsGrabbing = false;
                DestroyAllDots();
            }
            private void OnMousePress()
            {
                if (!CanGrab)
                {
                    return;
                }
                if (_currentKnob != null)
                {
                    //if not the two corners on the side
                    if (_currentKnob.CornerIndex == tvController.CurrentCorner()[0] || _currentKnob.CornerIndex == tvController.CurrentCorner()[1])
                    {
                        CanScoreLogo = true;
                        _currentKnob.OnGrab();
                        IsGrabbing = true;
                        LaunchDiscTrajectoryCopies();
                        tvController.CloseCorners();
                        tvController.ResetBounceCount();
                        DVManager.ResetPotentialScore();
                    }
                }
            }
            
            private void OnMouseHold()
            {
                if (!CanGrab)
                {
                    return;
                }
                if (!IsGrabbing)
                {
                    return;
                }
                IsSlow = true;
                IsMidSlow = false;
                _slowStickTick = slowStickTickMax;
                if (_trajectoryDisc != null && _currentDisc != null)
                {
                    if (Vector3.Distance(_lastPos, transform.position) > 0.01f) //min movement to reset trajectory
                    {
                        _currentDisc.ResetCopy();
                        DestroyAllDots();
                    }
                    else if (_trajectoryDisc.CanSetNewLife)
                    {
                        _currentDisc.ResetCopy();
                        DestroyAllDots();
                    }
                    _lastPos = transform.position;
                }
            }
            private void OnMouseRelease()
            {
                if (_currentKnob != null)
                {
                    IsSlow = false;
                    if(IsGrabbing)
                    {
                        IsMidSlow = true;
                    }
                    _currentKnob.OnRelease();
                    IsGrabbing = false;
                    DestroyAllDots();
                }
            }
            Vector2 GetRandomPointInPolygon(PolygonCollider2D collider)
            {
                Bounds bounds = collider.bounds;
                Vector2 randomPoint;
                int failCount = 0;
                int failCountMax = 100000;

                do
                {
                    failCount++;
                    if (failCount > failCountMax)
                    {
                        return (collider.transform.position);
                    }
                    randomPoint = new Vector2(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y)
                    );
                }
                while (!IsPointInPolygon(randomPoint, collider));

                return randomPoint;
            }

            bool IsPointInPolygon(Vector2 point, PolygonCollider2D collider)
            {
                Vector2 localPoint = collider.transform.InverseTransformPoint(point);
                return collider.OverlapPoint(localPoint);
            }


            public void DestroyAllDots()
            {
                foreach (Transform child in dotParent)
                {
                    Destroy(child.gameObject);
                }
            }
            private void LaunchDiscTrajectoryCopies()
            {
                foreach (Transform child in logoParent) //Optimize
                {
                    child.gameObject.GetComponent<LogoBehavior_DV>().LaunchTrajectoryCopy();
                }
            }

            private void ResetGame()
            {
                
            }
            /*private void OnControllerPress()
            {
            
            }*/
            /*private void OnControllerRelease()
            {
            
            }*/

            public void CloseGame()
            {
                tvController.ResetGame();
                ResetGame();
            }

        }
    }
}
