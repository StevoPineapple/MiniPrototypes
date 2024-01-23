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

            public DVDTaskBehavior DVManager;
            [SerializeField] private GameObject logoObject;
            [SerializeField] private GameObject _trajectoryDiscObject;
            [SerializeField] private Transform tvObjParent;
            [SerializeField] private Transform trajectoryObjParent;
            [SerializeField] private Transform dotParent;
            [SerializeField] private Transform detectorParent;

            private LogoBehavior_DV _currentDisc;
            private TrajectoryLogo_DV _trajectoryDisc;
            private Rigidbody2D _trajectoryDiscBody;
            private bool _isAiming;
            public bool IsSlow;
            private Vector3 _lastPos;

            [SerializeField] private SpriteShapeController ColliderSprShape;

            private KnobBehavior_DV _currentKnob;
            [SerializeField] private TvController_DV tvController;
            public bool IsGrabbing;

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
                tvController.StartGame();
                //ResetGame();
            }
            void createLogo() 
            {
                if (Input.GetMouseButtonDown(1))
                {
                    _isAiming = true;
                    _currentDisc = Instantiate(logoObject, transform.position, transform.rotation, tvObjParent).GetComponent<LogoBehavior_DV>();
                    _currentDisc.PlayerController = this;
                    _trajectoryDisc = _currentDisc.CreateLaunchCopy(tvObjParent, trajectoryObjParent, dotParent, transform.position - _currentDisc.transform.position, 10);

                    StartCoroutine(DoLaunchCopies());
                    _lastPos = transform.position;
                }
                if (Input.GetMouseButton(1))
                {
                    IsSlow = true;
                    //Time.timeScale = 0.1f;
                    //Time.fixedDeltaTime = Time.timeScale * 0.02f;
                    if (Vector3.Distance(_lastPos, transform.position) > 0.01f)
                    {
                        ResetAllTrajectoryCopies();
                        _trajectoryDisc.SetNewLife(_currentDisc.transform.position, transform.position - _currentDisc.transform.position);
                        foreach (Transform child in dotParent)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                    else if (_trajectoryDisc.CanSetNewLife)
                    {
                        foreach (Transform child in dotParent)
                        {
                            Destroy(child.gameObject);
                        }
                        ResetAllTrajectoryCopies();
                        _trajectoryDisc.SetNewLife(_currentDisc.transform.position, transform.position - _currentDisc.transform.position);
                    }
                    _currentDisc.SetAngle(transform.position - _currentDisc.transform.position);
                    _lastPos = transform.position;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    IsSlow = false;
                    //Time.timeScale = 1f;
                    //Time.fixedDeltaTime = Time.timeScale * 0.02f;
                    _isAiming = false;
                    _currentDisc.Launch();
                    StopAllCoroutines();
                    foreach (Transform child in dotParent)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }

            void DebugCreateLogo()
            {
                if (!DebugMode)
                {
                    return;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    _isAiming = true;
                    _currentDisc = Instantiate(logoObject, transform.position, transform.rotation, tvObjParent).GetComponent<LogoBehavior_DV>();
                    _currentDisc.PlayerController = this;
                    _trajectoryDisc = _currentDisc.CreateLaunchCopy(tvObjParent, trajectoryObjParent, dotParent, transform.position - _currentDisc.transform.position, 15);

                    StartCoroutine(DoLaunchCopies());
                    _lastPos = transform.position;
                }
                if (Input.GetMouseButton(1))
                {
                    //Time.timeScale = 0.1f;
                    //Time.fixedDeltaTime = Time.timeScale * 0.02f;
                    if (Vector3.Distance(_lastPos, transform.position) > 0.01f)
                    {
                        ResetAllTrajectoryCopies();
                        _trajectoryDisc.SetNewLife(_currentDisc.transform.position, transform.position - _currentDisc.transform.position);
                        foreach (Transform child in dotParent)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                    else if (_trajectoryDisc.CanSetNewLife)
                    {
                        foreach (Transform child in dotParent)
                        {
                            Destroy(child.gameObject);
                        }
                        ResetAllTrajectoryCopies();
                        _trajectoryDisc.SetNewLife(_currentDisc.transform.position, transform.position - _currentDisc.transform.position);
                    }
                    _currentDisc.SetAngle(transform.position - _currentDisc.transform.position);
                    _lastPos = transform.position;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    _isAiming = false;
                    _currentDisc.Launch();
                    StopAllCoroutines();
                    foreach (Transform child in dotParent)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            private void Update()
            {
                DebugCreateLogo();
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
                if (Input.GetKey(KeyCode.Space))
                {
                    IsSlow = true;
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    IsSlow = false;
                }
            }
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (IsGrabbing)
                {
                    return;
                }
                if (collision.CompareTag("Knob_DV"))
                {
                    _currentKnob = collision.GetComponent<KnobBehavior_DV>();
                }
            }
            private void OnTriggerExit2D(Collider2D collision)
            {
                if (IsGrabbing)
                {
                    return;
                }
                if (collision.CompareTag("Knob_DV"))
                {
                    _currentKnob = null;
                }
            }
            private void OnMousePress()
            {
                if (_currentKnob != null)
                {
                    _currentKnob.OnGrab();
                    IsGrabbing = true;
                    LaunchDiscTrajectoryCopies();
                }
            }
            private void OnMouseHold()
            {
                if (_trajectoryDisc != null && _currentDisc != null)
                {
                    if (Vector3.Distance(_lastPos, transform.position) > 0.01f)
                    {
                        ResetAllTrajectoryCopies();
                        //_trajectoryDisc.SetNewLife(_currentDisc.transform.position, transform.position - _currentDisc.transform.position);
                        foreach (Transform child in dotParent)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                    else if (_trajectoryDisc.CanSetNewLife)
                    {
                        foreach (Transform child in dotParent)
                        {
                            Destroy(child.gameObject);
                        }
                        ResetAllTrajectoryCopies();
                        //_trajectoryDisc.SetNewLife(_currentDisc.transform.position, transform.position - _currentDisc.transform.position);
                    }
                    //_currentDisc.SetAngle(transform.position - _currentDisc.transform.position);
                    _lastPos = transform.position;
                }
            }
            private void OnMouseRelease()
            {
                if (_currentKnob != null)
                {
                    _currentKnob.OnRelease();
                    IsGrabbing = false;
                    StopAllCoroutines();
                    foreach (Transform child in dotParent)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            public void DestroyAllDots()
            {
                foreach (Transform child in dotParent)
                {
                    Destroy(child.gameObject);
                }
            }
            private void ResetAllTrajectoryCopies()
            {
                foreach (Transform child in tvObjParent) //Optimize
                {
                    child.gameObject.GetComponent<LogoBehavior_DV>().ResetCopy();
                }
            }
            private void LaunchDiscTrajectoryCopies()
            {
                foreach (Transform child in tvObjParent) //Optimize
                {
                    child.gameObject.GetComponent<LogoBehavior_DV>().LaunchTrajectoryCopy();
                }
            }
            private void OnDrawGizmos()
            {
                Gizmos.color = Color.white;
                if (_isAiming)
                {
                    Gizmos.DrawLine(_currentDisc.transform.position, transform.position);
                }
            }
            void FixedUpdate()
            {
                
                if (DVManager.HasWon)
                {
                    return;
                }
                //Check Swipe
            }

            IEnumerator DoLaunchCopies()
            {
                yield return new WaitForFixedUpdate();
                LaunchDiscTrajectoryCopies();
            }
            private void ResetGame()
            {
                StopAllCoroutines();
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
