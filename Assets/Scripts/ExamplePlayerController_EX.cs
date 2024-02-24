using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RitualNight.Variables;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PlayerController_EX : MonoBehaviour
        {
            /*[Header("PartyControl")]
            [SerializeField] private BoolReference isMobile;
            [SerializeField] private PartyGameController PartyGameController;
            */
            
            public ExampleTaskBehavior EXManager;

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
            }
            void FixedUpdate()
            {
                //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (EXManager.HasWon)
                {
                    return;
                }
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
