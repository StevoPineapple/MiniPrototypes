using System.Collections;
using System.Collections.Generic;
using UnityEngine;
        public class PlayerController_RT : MonoBehaviour
        {
            public GameObject RingObject;
            public RingTossTaskBehavior RTManager;
            public RingObject_RT Ring;
            public float TossStrength;

            [Header("SwipeControl")]
            private Vector3 StartTossPos;
            [SerializeField] private Vector3 CurrTossPos = Vector3.zero;
            [SerializeField] private float TossTopSpeed;
            [SerializeField] private float TossTopSpeedAvg;
            [SerializeField] private float speedThreshold;
            private List<Vector3> _posSampleList = new List<Vector3>();
            [SerializeField] private int posSampleSize;
            private Vector3 EndTossPos;
            public SpriteRenderer SwipeRangeSprRend;
            [SerializeField] private bool WillThrow; //If over speed limit and will definitely throw
            public bool IsHolding;
            public bool CanToss = true;
            // Start is called before the first frame update
            private void OnEnable()
            {
                /*PartyGameController.OnPartyGameActionPress += MousePress;
                if (isMobile.Value)
                {
                    PartyGameController.OnPartyGameActionReleaseMobile += MouseRelease;
                }
                else
                {
                    PartyGameController.OnPartyGameActionRelease += MouseRelease;
                }*/
            }

            private void OnDisable()
            {
                /*PartyGameController.OnPartyGameActionPress -= MousePress;
                if (isMobile.Value)
                {
                    PartyGameController.OnPartyGameActionReleaseMobile -= MouseRelease;
                }
                else
                {
                    PartyGameController.OnPartyGameActionRelease -= MouseRelease;
                }
                */
            }
            void Start()
            {
                ResetTossStats();
            }

            // Update is called once per frame
            [SerializeField] private Vector3 _deltaPos;
            private void Update()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    MousePress();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    MouseRelease();
                }
            }
            void FixedUpdate()
            {
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (RTManager.HasWon)
                {
                    CanToss = false;
                    return;
                }
                if (!CanToss)
                {
                    return;
                }
                //Check Swipe
                if (IsHolding)
                {
                    if (!Ring.IsTossed)
                    {
                        _deltaPos = transform.position - CurrTossPos;
                        Vector3 _deltaPosAvg = CalculateDeltaPosAvg(_deltaPos);
                        float _speedThisFrame = Vector3.Distance(transform.position, CurrTossPos);
                        float _speedAvg = CalculateSpeedAvg(_speedThisFrame);
                        CurrTossPos = transform.position;
                        if (_speedAvg > speedThreshold) //speed of 1 frame
                        {
                            WillThrow = true;
                        }
                        if (WillThrow)
                        {
                            if (_speedAvg > TossTopSpeed)
                            {
                                TossTopSpeed = _speedAvg;
                            }
                            else
                            {
                                Ring.Toss(_deltaPosAvg.x, _deltaPosAvg.y);
                                ResetTossStats();
                            }
                        }
                        /*
                        Vector3 _deltaPos = EndTossPos - StartTossPos;
                        float _tossStrength = _deltaPos.y / (SwipeRangeSprRend.bounds.max.y - SwipeRangeSprRend.bounds.min.y);
                        print(_tossStrength);
                        float _sideStrength = _deltaPos.x / (SwipeRangeSprRend.bounds.max.x - SwipeRangeSprRend.bounds.min.x);
                        */
                        //Ring.Toss(_sideStrength, _tossStrength);
                    }
                }
            }
            [SerializeField] private int speedSampleSize;
            private List<float> _speedSampleList = new List<float>();
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
            private Vector3 CalculateDeltaPosAvg(Vector3 _currentDeltaPos)
            {
                _posSampleList.Insert(0, _currentDeltaPos); //add
                _posSampleList.RemoveAt(posSampleSize); //remove last
                Vector3 _posAvg = Vector3.zero;
                foreach (Vector3 sample in _posSampleList)
                {
                    _posAvg += sample;
                }
                _posAvg /= posSampleSize;
                return _posAvg;
            }
            private void ResetTossStats()
            {
                CanToss = true;
                IsHolding = false;
                _deltaPos = Vector3.zero;
                WillThrow = false;
                CurrTossPos = Vector3.zero;
                TossTopSpeed = 0;

                _posSampleList.Clear();
                _speedSampleList.Clear();
                for (int i = 0; i < posSampleSize; i++)
                {
                    _posSampleList.Add(Vector3.zero);
                }
                for (int i = 0; i < speedSampleSize; i++)
                {
                    _speedSampleList.Add(0);
                }
            }
            private void MousePress()
            {
                CurrTossPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                StartTossPos = transform.position;
                IsHolding = true;
            }
            private void MouseRelease()
            {
                //CurrTossPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                EndTossPos = transform.position;
                IsHolding = false;

                ResetTossStats();
            }
            public void CloseGame()
            {
                ResetTossStats();
                CanToss = false;
            }

        }