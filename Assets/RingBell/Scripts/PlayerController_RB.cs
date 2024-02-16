using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RitualNight.Variables;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PlayerController_RB : MonoBehaviour
        {
            /*[Header("PartyControl")]
            [SerializeField] private BoolReference isMobile;
            [SerializeField] private PartyGameController PartyGameController;
            */
            
            public RingBellTaskBehavior RBManager;
            [SerializeField] private ParallaxManager_RB paraManager;
            [SerializeField] private LaunchBar_RB launchBar;

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
                if (RBManager.HasWon)
                {
                    return;
                }
                //Check Swipe
            }
            
            private void ResetGame()
            {
            
            }
            /*private void ControllerPress()
            {
            
            }*/
            /*private void ControllerRelease()
            {
            
            }*/

            private void MousePress()
            {
                launchBar.OnMousePress();
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
