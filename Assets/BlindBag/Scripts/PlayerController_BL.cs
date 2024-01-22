using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RitualNight.Variables;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PlayerController_BL : MonoBehaviour
        {
            /*[Header("PartyControl")]
            [SerializeField] private BoolReference isMobile;
            [SerializeField] private PartyGameController PartyGameController;
            */

            [Header("Components")]
            [SerializeField] private BlindBagTaskBehavior BLManager;
            [SerializeField] private ObjectDetector_BL ObjectDetector;
            [SerializeField] private ExpressionController_BL Expression;

            [Header ("Cursor")]
            private Vector3 _lastPos;
            [SerializeField] private SpriteRenderer CursorSprRend;
            private int ShowTick;
            [SerializeField] private int ShowTickMax;
            private CircleCollider2D _selfCollider;
            private bool _isTouching;

            [SerializeField] private float checkSpeedMin;
            [SerializeField] private int speedSampleSize;

            [SerializeField] private float checkLerpSpeed;
            [SerializeField] private float checkLerpSpeedOnObject;

            private List<float> _speedSampleList = new List<float>();
            private float _speedAvg;

            [Header("Bag")]
            [SerializeField] private ObjectJumpy_BL BagJumpy;
            [SerializeField] private Animator BagAnimator;

            [Header("Foot")]
            private bool _isFootInBag;
            [SerializeField] private Animator FootAnim;
            [SerializeField] private AnimationClip FoorEnterClip;

            [Header("Hair")]
            [HideInInspector] public Transform PrizeTransform;
            [SerializeField] private Animator HairAnim;
            [SerializeField] private float BlinkSpeedAdj;
            [SerializeField] private float BlinkSpeedMax;
            [SerializeField] private float BlinkSpeedMin;
            [SerializeField] private float BlinkMaxDistance; //threshold for blink to start change speed

            [Header("Hint")]
            [SerializeField] private Transform ArrowInitPos;
            [SerializeField] private GameObject ArrowObject;


            

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
                _selfCollider = GetComponent<CircleCollider2D>();
                ResetGame();
                BagAnimator.speed = 0;
                CursorSprRend.enabled = false;
                ShowTick = ShowTickMax;
                CursorSprRend.transform.position = transform.position;

                ArrowObject.SetActive(true);

                _selfCollider.enabled = true;
                //Cursor.visible = false;

            }
            private float CalculateSpeedAvg(float _currentSpeed)
            {
                _speedSampleList.Insert(0, _currentSpeed); //add
                _speedSampleList.RemoveAt(speedSampleSize); //remove last
                float _speedAvg = 0;
                foreach (float sample in _speedSampleList)
                {
                    _speedAvg += sample;
                }
                _speedAvg /= speedSampleSize;
                return _speedAvg;
            }
            private void FixedUpdate()
            {
                if (BLManager.HasWon)
                {
                    return;
                }

                //Move cursor
                if (ObjectDetector.IsOnObject)
                {
                    transform.position = Vector3.Lerp(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), checkLerpSpeedOnObject);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), checkLerpSpeed);
                }
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);

                if (!_isFootInBag)
                {
                    ArrowObject.transform.position = ArrowInitPos.position + new Vector3(Mathf.Sin(Time.time * 3) * 0.1f, 0, 0);
                    CursorSprRend.enabled = true;
                    return;
                }
                
                BlinkingLight();

                if (ObjectDetector.IsOnBag)
                {
                    CursorSprRend.transform.position = _lastPos;
                }

                //Manage Checking
                _speedAvg = CalculateSpeedAvg((transform.position - _lastPos).magnitude);
                if (_speedAvg > checkSpeedMin)
                {
                    BagAnimator.speed = _speedAvg*3;
                    Expression.EggSearching();
                    _selfCollider.enabled = false;
                    ShowTick = ShowTickMax;
                    _isTouching = false;
                }
                else
                {
                    BagAnimator.speed = 0;
                    if (ShowTick <= 0) ///Check optimize;
                    {
                        _isTouching = true;
                        if (ObjectDetector.IsOnObject)
                        {
                            _selfCollider.enabled = true;
                        }
                    }
                    else
                    {
                        if (ObjectDetector.IsOnObject&&!_isTouching)
                        {
                            Expression.EggChecking();
                        }
                        ShowTick--;
                    }
                }
                _lastPos = transform.position;
                //PartyGameController.PartyGameFollow(transform, PartyGameController.IsUsingController ? 5 : 100);
            }

            private void BlinkingLight()
            {
                float _distanceToPrize = BlinkMaxDistance - Mathf.Abs((CursorSprRend.transform.position - PrizeTransform.position).magnitude);
                print(Mathf.Clamp(_distanceToPrize * BlinkSpeedAdj, BlinkSpeedMin, BlinkSpeedMax));
                HairAnim.speed = Mathf.Clamp(_distanceToPrize * BlinkSpeedAdj, BlinkSpeedMin, BlinkSpeedMax);
            }

            void Update()
            {
                if (BLManager.HasWon)
                {
                    return;
                }

                //Check Swipe
            }

            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (!_isFootInBag)
                {
                    if (collision.CompareTag("BagStart_BL")) //Game Start Foot In
                    {
                        _selfCollider.enabled = false;
                        _isFootInBag = true;
                        BagJumpy.Jump();
                        Expression.gameObject.SetActive(true);
                        FootAnim.Play("FootEnter");
                        CursorSprRend.enabled = true;
                        ArrowObject.SetActive(false);
                    }
                }
                else
                {
                    if (collision.CompareTag("Prize_BL"))
                    {
                        Expression.EggPrizeWin();
                        FootAnim.Play("FootWin");
                        BLManager.SetWin();
                    }
                    else if (collision.CompareTag("Normal_BL"))
                    {
                        Expression.EggShaking();
                    }
                    else if (collision.CompareTag("Gross_BL"))
                    {
                        Expression.EggGross();
                    }
                }
            }
            private void ResetGame()
            {
                for (int i = 0; i < speedSampleSize; i++)
                {
                    _speedSampleList.Add(0);
                }
                _selfCollider.enabled = false;
                _isFootInBag = false;
                _isTouching = false;
                

                //Cursor.visible = true;
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
