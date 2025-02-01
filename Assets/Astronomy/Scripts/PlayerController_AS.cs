using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RitualNight.Variables;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PlayerController_AS : MonoBehaviour
        {
            /*[Header("PartyControl")]
            [SerializeField] private BoolReference isMobile;
            [SerializeField] private PartyGameController PartyGameController;
            */
            
            public AstronomyTaskBehavior ASManager;
            private StarBehavior_AS currStar;
            private bool _canInteract;
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
                //print(ASManager.CheckWithObj(gameObject));
            }
            void FixedUpdate()
            {
                if (ASManager.HasWon)
                {
                    return;
                }
            }
            
            private void ResetGame()
            {
                _canInteract = true;
                currStar = null;
            }
            /*private void ControllerPress()
            {
            
            }*/
            /*private void ControllerRelease()
            {
            
            }*/
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (!_canInteract)
                {
                    return;
                }
                if (collision.CompareTag("Star_AS"))
                {
                    if (currStar != null)
                    {
                        currStar.OnLeaveHover();
                    }
                    currStar = collision.GetComponent<StarBehavior_AS>();
                    currStar.OnHover();
                }
            }
            private void OnTriggerExit2D(Collider2D collision)
            {
                if (!_canInteract)
                {
                    return;
                }
                if (currStar != null)
                {
                    if (collision.gameObject == currStar.gameObject)
                    {
                        currStar.OnLeaveHover();
                        currStar = null;
                    }
                }
            }

            private void OnMousePress()
            {
                if (currStar == null)
                {
                    return;
                }
                if (!currStar.HasClicked)
                {
                    currStar.OnClick();
                    if (currStar.IsGood)
                    {
                        ASManager.OnGoodStarClick();
                    }
                }
            }
            private void OnMouseRelease()
            {
            
            }
            public void CloseGame()
            {
                ResetGame();
            }
            public void SetCannotInteract()
            {
                _canInteract = false;
                if (currStar != null)
                {
                    currStar.OnLeaveHover();
                }
                currStar = null;
            }
        }
    }
}
