using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RitualNight.Variables;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PlayerController_CRD : MonoBehaviour
        {
            /*[Header("PartyControl")]
            [SerializeField] private BoolReference isMobile;
            [SerializeField] private PartyGameController PartyGameController;
            */

            [Header("Selection")]
            private bool _canSelect;
            private bool _isOnButton;
            [SerializeField] private PartyGameEnlarge button;

            public CardsTaskBehavior CRDManager;
            private CardBehavior_CRD _currentCard;
            private Collider2D _selfCollider;
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
            private void Awake()
            {
                _selfCollider = GetComponent<CircleCollider2D>();
            }
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
            public void ColliderEnable(bool value)
            {
                _selfCollider.enabled = value;
            }
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision.CompareTag("Card_CRD")&&_canSelect)
                {
                    _currentCard = collision.GetComponent<CardBehavior_CRD>();
                    _currentCard.SetEdgeVisible(true);
                }
                else if (collision.CompareTag("Button_CRD"))
                {
                    _isOnButton = true;
                    button.Enlarge();
                }
            }
            private void OnTriggerExit2D(Collider2D collision)
            {
                if (collision.CompareTag("Card_CRD") && _canSelect)
                {
                    if (_currentCard != null)
                    {
                        _currentCard.SetEdgeVisible(false);
                        _currentCard = null;
                    }
                }
                else if (collision.CompareTag("Button_CRD"))
                {
                    _isOnButton = false;
                    button.ResetSize();
                }
            }
            void FixedUpdate()
            {
                
                if (CRDManager.HasWon)
                {
                    return;
                }
            }

            public void SetCanSelect(bool _canSelect)
            {
                this._canSelect = _canSelect;
            }
            public void ResetGame()
            {
                _canSelect = false;
                _isOnButton = false;
                _currentCard = null;
                ColliderEnable(true);
            }
            /*private void ControllerPress()
            {
            
            }*/
            /*private void ControllerRelease()
            {
            
            }*/

            private void OnMousePress()
            {
                if (_isOnButton)
                {
                    _isOnButton = false;
                    CRDManager.EndMemoryPhase();
                    return;
                }
            }
            private void OnMouseRelease()
            {
                if (!_canSelect)
                {
                    return;
                }
                if (_currentCard != null)
                {
                    _currentCard.FlipToFront();
                    _currentCard.SetEdgeVisible(false);
                    _currentCard.DisableSelf();
                    ColliderEnable(false);
                    CRDManager.RevealCard(_currentCard);
                    _currentCard = null;
                }
            }
            public void CloseGame()
            {
                ResetGame();
            }

        }
    }
}
