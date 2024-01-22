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
            private LogoBehavior_DV _currentDisc;
            private TrajectoryLogo_DV _trajectoryDisc;
            private Rigidbody2D _trajectoryDiscBody;
            private bool _isAiming;
            public bool IsSlow;
            private Vector3 _lastPos;

            [SerializeField] private SpriteShapeController ColliderSprShape;
            /*private void OnEnable()
            {
                if (PartyGameController.IsUsingController)
                {
                    PartyGameController.OnPartyGameActionPress += ControllerPress;
                    PartyGameController.OnPartyGameActionRelease += ControllerRelease;
                }
                else
                {
                    PartyGameController.OnPartyGameActionPress += MousePress;
                    if (isMobile.Value)
                    {
                        PartyGameController.OnPartyGameActionReleaseMobile += MouseRelease;
                    }
                    else
                    {
                        PartyGameController.OnPartyGameActionRelease += MouseRelease;
                    }
                }
            }

            private void OnDisable()
            {
                if (PartyGameController.IsUsingController)
                {
                    PartyGameController.OnPartyGameActionPress -= ControllerPress;
                    PartyGameController.OnPartyGameActionRelease -= ControllerRelease;
                }
                else
                {
                    PartyGameController.OnPartyGameActionPress -= MousePress;
                    if (isMobile.Value)
                    {
                        PartyGameController.OnPartyGameActionReleaseMobile -= MouseRelease;
                    }
                    else
                    {
                        PartyGameController.OnPartyGameActionRelease -= MouseRelease;
                    }
                }
            }*/
            public void StartGame()
            {
                ResetGame();
            }
            void createLogo()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _isAiming = true;
                    _currentDisc = Instantiate(logoObject, transform.position, transform.rotation, tvObjParent).GetComponent<LogoBehavior_DV>();
                    _currentDisc.PlayerController = this;
                    _trajectoryDisc = _currentDisc.CreateLaunchCopy(tvObjParent, trajectoryObjParent, dotParent, transform.position - _currentDisc.transform.position, 10);

                    StartCoroutine(DoLaunchCopies());
                    _lastPos = transform.position;
                }
                if (Input.GetMouseButton(0))
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
                if (Input.GetMouseButtonUp(0))
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
            private void Update()
            {
                //PartyGameController.PartyGameFollow(transform, PartyGameController.IsUsingController ? 5 : 100);
                if (Input.GetMouseButtonDown(0))
                {
                   
                }
                if (Input.GetMouseButton(0))
                {
                   
                }
                if (Input.GetMouseButtonUp(0))
                {
                   
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
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
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
            /*private void ControllerPress()
            {
            
            }*/
            /*private void ControllerRelease()
            {
            
            }*/

            private void MousePress()
            {

            }
            private void MouseRelease()
            {

            }
            public void CloseGame()
            {
                ResetGame();
            }

        }
    }
}
