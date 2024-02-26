using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RitualNight.Variables;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PlayerController_TR : MonoBehaviour
        {
            /*[Header("PartyControl")]
            [SerializeField] private BoolReference isMobile;
            [SerializeField] private PartyGameController PartyGameController;
            */
            
            public TrolleyTaskBehavior TRManager;
            public bool IsSelecting;
            [SerializeField] private Animator carAnim;

            [Header("See Value")]
            public int SelectedTrack = 0; //0 down, 1 mid, 2 up

            [Header("Arrow")]
            [SerializeField] private SpriteRenderer[] arrowArr;
            [SerializeField] private float arrowBlinkTime;
            private bool _isArrowBlack;
            
            

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
            public void StartSelecting()//in timeline
            {                
                IsSelecting = true;
                StartCoroutine(BlinkArrow());
            }
            public void EndSelecting()//in timeline
            {
                TRManager.IsCamFollow = true;
                IsSelecting = false;


                carAnim.SetBool("isFirstTrack", true);
                carAnim.SetInteger("selectedTrack", SelectedTrack);
            }
            public void ReachSecondTrack()
            {
                carAnim.SetBool("isSecondTrack", true);
                carAnim.SetBool("isFirstTrack", false);
                carAnim.SetInteger("secondTrack", TRManager.ChangeTrackResultArr[SelectedTrack]);
            }
            public void EndSecondTrack()
            {
                carAnim.SetBool("isSecondTrack", false);
                carAnim.SetBool("isMunching", true);
                TRManager.MunchItem();
            }
            public void StartGame()
            {
                ResetGame();
            }
            private void Update()
            {
                //PartyGameController.PartyGameFollow(transform, PartyGameController.IsUsingController ? 5 : 100);
                if (Input.GetMouseButtonDown(0))
                {
                    MousePress();
                }
            }
            void FixedUpdate()
            {
                //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (TRManager.HasWon)
                {
                    return;
                }
            }
            
            private void ResetGame()
            {
                foreach (SpriteRenderer _sprRend in arrowArr)
                {
                    _sprRend.color = Color.white;
                }

                StopAllCoroutines();
                carAnim.SetBool("isFirstTrack", false);
                carAnim.SetBool("isSecondTrack", false);
                carAnim.SetBool("isWin", false);
                carAnim.SetBool("isMunching", false);
                SelectedTrack = 0;
            }
            /*private void ControllerPress()
            {
            
            }*/
            /*private void ControllerRelease()
            {
            
            }*/
            IEnumerator BlinkArrow()
            {
                while (IsSelecting)
                {
                    if (_isArrowBlack)
                    {
                        arrowArr[SelectedTrack].color = Color.black;
                    }
                    else
                    {
                        arrowArr[SelectedTrack].color = Color.gray;
                    }
                    _isArrowBlack = !_isArrowBlack;
                    yield return new WaitForSeconds(arrowBlinkTime);
                }
                foreach (SpriteRenderer _sprRend in arrowArr)
                {
                    _sprRend.color = Color.white;
                }
                arrowArr[SelectedTrack].color = Color.black;
            }
            private void MousePress()
            {
                foreach (SpriteRenderer _sprRend in arrowArr)
                {
                    _sprRend.color = Color.white;
                }
                SelectedTrack = (SelectedTrack+1)% 3;
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
