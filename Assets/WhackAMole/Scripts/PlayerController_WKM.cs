using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using RitualNight.Variables;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PlayerController_WKM : MonoBehaviour
        {
            /*[Header("PartyControl")]
            [SerializeField] private BoolReference isMobile;
            [SerializeField] private PartyGameController PartyGameController;
            */
            
            public WhackAMoleTaskBehavior WKMManager;
            [SerializeField] private int clickBufferFrame;
            private int _clickBufferCount;
            private bool _hasClicked;

            [Header("Sprite")]
            [SerializeField] private Sprite sprHammerUp;

            [SerializeField] private Collider2D selfCollider;
            [SerializeField] private SpriteRenderer spriteRenderer;
            private Animator _anim;
            private bool _canHit;
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
                _anim = GetComponent<Animator>();
                //selfCollider = GetComponent<Collider2D>();
                selfCollider.enabled = false;

            }
            public void StartGame()
            {
                _hasClicked = false;
                spriteRenderer.sprite = sprHammerUp;
                Cursor.visible = false;
                _canHit = true;
                selfCollider.enabled = false;
                ResetGame();
            }
            private void Update()
            {
                //PartyGameController.PartyGameFollow(transform, PartyGameController.IsUsingController ? 5 : 100);
                //if (_canHit)
                //{
                    transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                //}
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
                if (WKMManager.HasWon)
                {
                    return;
                }
                if (_clickBufferCount > 0)
                {
                    _clickBufferCount--;
                    if (_canHit && _hasClicked)
                    {
                        OnMousePress();
                    }
                }
                else
                {
                    _hasClicked = false;
                }
            }
            
            private void ResetGame()
            {
            Cursor.visible = true;
            }
            /*private void ControllerPress()
            {
            
            }*/
            /*private void ControllerRelease()
            {
            
            }*/
            private void OnMousePress()
            {
                if (_canHit)
                {
                    _anim.StopPlayback();
                    _anim.Play("HammerHit", -1, 0f);
                    transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                }
                else 
                {
                    _clickBufferCount = clickBufferFrame;
                    _hasClicked = true;
                }
            }
            private void OnMouseRelease()
            {
                
            }

            private void SetCanHitFalse()//called in animation
            {
                _canHit = false;
            }
            private void SetCanHit()//called in animation
            {
                _canHit = true;
            }

            private void OnTriggerEnter2D(Collider2D collision)//Golden add point logic is in the mole
            {
                if (collision.CompareTag("Mole_WKM"))
                {
                    DisableCollider();
                }
            }
            void HammerCheck()//called in animation
            {
                selfCollider.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);//
                selfCollider.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                selfCollider.enabled = true;
            }
            private void DisableCollider()//called in animation
            {
                selfCollider.enabled = false;
            }
            public void CloseGame()
            {
                ResetGame();
            }
            private void OnDisable()
            {
                Cursor.visible = true;
            }
            private void OnDestroy()
            {
                Cursor.visible = true;
            }
        }
    }
}
