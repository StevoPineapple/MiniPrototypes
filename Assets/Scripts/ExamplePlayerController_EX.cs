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
            /*
            private void OnEnable()
            {
                if (PartyGameController.IsUsingController)
                {
                    PartyGameController.OnPartyGameActionPress += ControllerPress;
                    PartyGameController.OnPartyGameActionRelease += ControllerRelease;
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
                    PartyGameController.OnPartyGameActionPress -= ControllerPress;
                    PartyGameController.OnPartyGameActionRelease -= ControllerRelease;
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
                ResetGame();
            }
            private void Update()
            {
                //PartyGameController.PartyGameFollow(transform, PartyGameController.IsUsingController ? 5 : 100);
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Input.GetMouseButtonDown(0))
                {
                    OnMousePress();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    OnMouseRelease();
                }
            }
            void FixedUpdate()
            {
                
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

            private void OnMousePress()
            {
            
            }
            private void OnMouseRelease()
            {
            
            }
            public void CloseGame()
            {
                ResetGame();
            }

        }
    }
}
