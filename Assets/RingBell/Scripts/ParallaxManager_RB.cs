using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
namespace RitualNight
{
    namespace PartyGames
    {
        [ExecuteInEditMode]
        public class ParallaxManager_RB : MonoBehaviour
        {
            public RingBellTaskBehavior RBManager;
            public ScrollCameraManager_RB CamManager;
            [SerializeField] private LaunchBar_RB launchBar;
            public bool DebugMode;
            public bool AllViewMode;
            public bool AllSetPosInView;
            private bool _hasReturnToCamStart;
            public bool AllSetOutsideMask;
            public bool AllSetNoneMask;

            [Header("Height")]
            public bool IsOK;
            [SerializeField] private Transform topTrans;
            [SerializeField] private Transform okBellTrans;
            
            [SerializeField] private SpriteRenderer heightLineSprRend;
            [SerializeField] private TextMeshProUGUI heightText;
            private bool _isHeightSet;
            [SerializeField] private float heightDisplayMinSpd;
            [SerializeField] private float heightPlusAdj;
            [SerializeField] private float heightMultAdj;

            [Header("Slidey")]
            public bool IsLaunched;
            private bool _isMoveSlidey;
            [SerializeField] private Transform SlideyInitTrans;
            [SerializeField] private GameObject SlideyObject;
            [SerializeField] private float gravity;
            [SerializeField] private float slideyDelayMax;
            [SerializeField] private float slideyDelayAdj;
            [SerializeField] private float slideyDelayMinSpd;
            [SerializeField] private AnimationCurve gravityCurve;

            [Header ("Values")]
            [SerializeField] private float currSpeed;
            [SerializeField] private float launchSpeed;
            [SerializeField] private float maxUpSpeed;

            [Header ("Tune")]
            [SerializeField] private AnimationCurve launchSpeedAdjCurve;
            [SerializeField] private AnimationCurve upSpeedCurve;
            [SerializeField] private float clampMinUpSpeed;
            [SerializeField] private float clampMaxUpSpeed;
            [SerializeField] private float minLaunchSpeed;
            [SerializeField] private float maxLaunchSpeed;
            [SerializeField] private float errorPointsAdj;
            [SerializeField] private float launchToUpSpeedRatio;

            [Header ("Bell")]
            [SerializeField] private GameObject topBellObject;
            [SerializeField] private Transform topBellObjectInitTrans;
            [SerializeField] private float breakBellMinSpd;
            [SerializeField] private GameObject fireworkParticleObject;
            private bool _brokenBell;
            private bool _hitBell;

            [Header("Void")]
            [SerializeField] private GameObject voidInner1;
            [SerializeField] private GameObject voidInner2;
            [SerializeField] private GameObject voidInner3;
            [SerializeField] private float voidSpeed;


            [Header ("AutoMotion")]
            public bool AutoMotion;
            public float Amp;
            public float Frequency;
            public float Offset;

            private bool _isAutoMotionInit;
            private Vector3 _autoMotionInitPos;

            [Header ("Debug")]
            //Use this as a workaround to the para object pos shifting bug
            [SerializeField] private bool debugDiscardParaObjInitPos;
            [SerializeField] private Transform debugHeightTextPos;
            public float debugGetRawLaunchSpeed;
            public float debugGetErrorAmount;

#if UNITY_EDITOR
            private void OnEnable()
            {
                EditorApplication.update += UpdateInEditor;
                EditorApplication.QueuePlayerLoopUpdate();
            }

            private void OnDisable()
            {
                EditorApplication.update -= UpdateInEditor;
            }

            private void UpdateInEditor() //this is for auto motion
            {
                // Your continuous update logic goes here

                // Repaint the hierarchy to force Gizmos to update every frame
                EditorApplication.RepaintHierarchyWindow();
                // Queue the next player loop update
                EditorApplication.QueuePlayerLoopUpdate();
            }


            private void OnDrawGizmosSelected() //debug mode
            {
                if (!DebugMode)
                {
                    foreach (Transform _child in transform)
                    {
                        if (_child.tag != "Static_RB")
                        {
                            if (_child.tag != "Parent_RB")
                            {
                                _child.GetComponent<ParallaxObject_RB>().DebugMode = false;
                            }
                            else
                            {
                                foreach (Transform _nestedChild in _child)
                                {
                                    _nestedChild.GetComponent<ParallaxObject_RB>().DebugMode = false;
                                }
                            }
                        }
                    }
                    return;
                }

                foreach (Transform _child in transform)
                {
                    if (_child.tag != "Static_RB")
                    {
                        if (_child.tag != "Parent_RB")
                        {
                            _child.GetComponent<ParallaxObject_RB>().SetViewMode(AllViewMode);
                        }
                        else
                        {
                            foreach (Transform _nestedChild in _child)
                            {
                                _nestedChild.GetComponent<ParallaxObject_RB>().SetViewMode(AllViewMode);
                            }
                        }
                    }
                }
                if (AllSetPosInView)
                {
                    foreach (Transform _child in transform)
                    {
                        if (_child.tag != "Static_RB")
                        {
                            if (_child.tag != "Parent_RB")
                            {
                                _child.GetComponent<ParallaxObject_RB>().SetPosInViewMode(true);
                            }
                            else
                            {
                                foreach (Transform _nestedChild in _child)
                                {
                                    _nestedChild.GetComponent<ParallaxObject_RB>().SetPosInViewMode(true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (Transform _child in transform)
                    {
                        if (_child.tag != "Static_RB")
                        {
                            if (_child.tag != "Parent_RB")
                            {
                                _child.GetComponent<ParallaxObject_RB>().SetPosInViewMode(false);
                            }
                            else
                            {
                                foreach (Transform _nestedChild in _child)
                                {
                                    _nestedChild.GetComponent<ParallaxObject_RB>().SetPosInViewMode(false);
                                }
                            }
                        }
                    }
                }
                if (AllSetOutsideMask)
                {
                    foreach (Transform _child in transform)
                    {
                        if (_child.tag != "Static_RB")
                        {
                            if (_child.tag != "Parent_RB")
                            {
                                _child.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                            }
                            else
                            {
                                foreach (Transform _nestedChild in _child)
                                {
                                    _nestedChild.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                                }
                            }
                        }
                    }
                    AllSetOutsideMask = false;
                }
                if (AllSetNoneMask)
                {
                    foreach (Transform _child in transform)
                    {
                        if (_child.tag != "Static_RB")
                        {
                            if (_child.tag != "Parent_RB")
                            {
                                _child.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                            }
                            else
                            {
                                foreach (Transform _nestedChild in _child)
                                {
                                    _nestedChild.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                                }
                            }
                        }
                    }
                    AllSetNoneMask = false;
                }
            }
            private void OnDrawGizmos()
            {
                if (!DebugMode)
                {
                    return;
                }


                if (debugDiscardParaObjInitPos)
                {
                    foreach (Transform _child in transform)
                    {
                        if (_child.tag != "Static_RB")
                        {
                            if (_child.tag != "Parent_RB")
                            {
                                _child.GetComponent<ParallaxObject_RB>().DiscardInitPos();
                            }
                            else
                            {
                                foreach (Transform _nestedChild in _child)
                                {
                                    _nestedChild.GetComponent<ParallaxObject_RB>().DiscardInitPos();
                                }
                            }
                        }
                    }
                    return;
                }
                if (AllViewMode)
                {
                    _hasReturnToCamStart = false;
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.yellow;
                    style.fontSize = 30;

                    Handles.Label(transform.position, "VIEW MODE", style);
                    style.fontSize = 20;
                    Handles.Label(debugHeightTextPos.position, ((-transform.position.y+heightPlusAdj)*heightMultAdj).ToString(), style);

                    if (AutoMotion)
                    {
                        if (!_isAutoMotionInit)
                        {
                            _autoMotionInitPos = transform.position;
                            _isAutoMotionInit = true;
                        }
                        transform.position = new Vector3(transform.position.x, _autoMotionInitPos.y + Offset + Mathf.Sin(Time.time * Frequency) * Amp, 0);
                    }
                    else
                    {
                        if (_isAutoMotionInit)
                        {
                            transform.position = _autoMotionInitPos;
                            _isAutoMotionInit = false;
                        }
                        
                    }
                }
                else if (!_hasReturnToCamStart)
                {
                    transform.position = CamManager.StartPos.position;
                    _hasReturnToCamStart = true;
                }
            }//Debug mode
#endif
            private void Update()
            {
                return;
                if (Input.GetKeyDown(KeyCode.R)&&DebugMode)
                {
                    ResetGame();
                }

                if (IsLaunched)
                {
                    if (okBellTrans.position.y < SlideyObject.transform.position.y)
                    {
                        IsOK = true;
                    }

                    float _fps = Application.targetFrameRate > -1 ? Application.targetFrameRate : 60; //Used to keep the current speed tune
                    currSpeed -= gravityCurve.Evaluate(currSpeed)*Time.deltaTime*_fps;
                    if (currSpeed <= heightDisplayMinSpd)
                    {
                        ShowHeight();   
                    }
                    if (!_isMoveSlidey)
                    {
                        transform.position += new Vector3(0, -Mathf.Clamp(currSpeed * Time.deltaTime * _fps, -999, maxUpSpeed), 0);
                        SlideyObject.transform.position = new Vector3(0, Mathf.Pow(Mathf.Clamp(currSpeed-slideyDelayMinSpd,0,slideyDelayMax),2)*slideyDelayAdj,0);
                    }
                    else
                    {
                        SlideyObject.transform.position -= new Vector3(0, -Mathf.Clamp(currSpeed * Time.deltaTime * _fps, -999, maxUpSpeed), 0);
                    }
                }
            }
            private void FixedUpdate()
            {
                if (Input.GetKeyDown(KeyCode.R) && DebugMode)
                {
                    ResetGame();
                }

                if (IsLaunched)
                {
                    //voidInner1.transform.Rotate(0, 0, voidSpeed);
                    voidInner2.transform.Rotate(0, 0, voidSpeed);
                    voidInner2.transform.Rotate(0, 0, voidSpeed*0.7f);
                    if (okBellTrans.position.y < SlideyObject.transform.position.y)
                    {
                        IsOK = true;
                        if (_brokenBell)
                        {
                            topBellObject.transform.position = SlideyObject.transform.position;
                        }
                        else if (topBellObject.transform.position.y < SlideyObject.transform.position.y)
                        {
                            if (currSpeed > breakBellMinSpd)
                            {
                                StartCoroutine(DoBrokenBell());
                                _brokenBell = true;
                            }
                            else //Hit the bell;
                            {
                                _hitBell = true;
                                SlideyObject.transform.position = topBellObject.transform.position;
                            }
                        }

                    }

                    float _fps = Application.targetFrameRate > -1 ? Application.targetFrameRate : 60; //Used to keep the current speed tune
                    currSpeed -= gravityCurve.Evaluate(currSpeed);
                    if (currSpeed <= heightDisplayMinSpd && !_brokenBell)
                    {
                        ShowHeight();
                    }
                    if (!_isMoveSlidey)//normal
                    {
                        if (!_hitBell)
                        {
                            //if hit bell, stay until speed turns to 0
                            transform.position += new Vector3(0, -Mathf.Clamp(currSpeed, 0, maxUpSpeed), 0);
                        }
                        SlideyObject.transform.position = new Vector3(0, Mathf.Pow(Mathf.Clamp(currSpeed - slideyDelayMinSpd, 0, slideyDelayMax), 2) * slideyDelayAdj, 0);
                    }
                    else//slidey fall
                    {
                        SlideyObject.transform.position -= new Vector3(0, -Mathf.Clamp(currSpeed, -999, maxUpSpeed), 0);
                    }
                }
            }
            IEnumerator DoBrokenBell()
            {
                yield return new WaitForSeconds(0.5f);
                fireworkParticleObject.SetActive(true);
                topBellObject.gameObject.SetActive(false);
                SlideyObject.gameObject.SetActive(false);
                SetHeightBrokenBell();
            }
            private void ShowHeight()
            {
                if (_isHeightSet)
                {
                    return;
                }
                float _height = ((-transform.position.y + heightPlusAdj) * heightMultAdj);
                float fadePercent = 1 - (Mathf.Clamp(currSpeed-0.5f,0,999) / heightDisplayMinSpd); //-0.5 to have more opaque time
                if (currSpeed <= 0)
                {
                    SetHeight();
                }
                heightText.text = _height.ToString();
                heightText.color = new Color(1, 1, 1, fadePercent);
                heightLineSprRend.color = new Color(1, 1, 1, fadePercent);
            }
            private void SetHeight()
            {
                _isMoveSlidey = true;
                _isHeightSet = true;
                float _height = ((-transform.position.y + heightPlusAdj) * heightMultAdj);
                heightText.text = _height.ToString();
                heightText.color = Color.white;
                heightLineSprRend.color = Color.white;
                float _score = Mathf.Clamp01(Mathf.Abs((SlideyObject.transform.position.y - okBellTrans.position.y) / (SlideyObject.transform.position.y - topTrans.position.y)));

                if (IsOK)
                {
                    launchBar.ShowResult(RBManager.GetReturnTime(_score) * 0.5f);//0.5 so there is time between show and end
                }

                RBManager.WinCheck(IsOK, _score);
            }

            private void SetHeightBrokenBell()
            {
                float _score = Mathf.Clamp01(Mathf.Abs((SlideyObject.transform.position.y - okBellTrans.position.y) / (SlideyObject.transform.position.y - topTrans.position.y)));
                RBManager.WinCheck(IsOK, _score+0.2f);//score bonus to close faster
            }

            public void StartGame()
            {
                ResetGame();
            }
            public void ResetGame()
            {
                StopAllCoroutines();
                topBellObject.gameObject.SetActive(true);
                topBellObject.transform.position = topBellObjectInitTrans.position;

                SlideyObject.gameObject.SetActive(true);
                SlideyObject.transform.position = SlideyInitTrans.position;

                fireworkParticleObject.SetActive(false);

                _hitBell = false;
                _brokenBell = false;

                IsOK = false;
                _isMoveSlidey = false;
                IsLaunched = false;
                currSpeed = 0;
                heightText.color = new Color(1,1,1,0);
                heightLineSprRend.color = new Color(1, 1, 1, 0);
                _isHeightSet = false;
                transform.position = CamManager.StartPos.position;
            }

            public void Launch(float _errorAmount)
            {
                IsLaunched = true;
                float _rawLaunchSpeed = maxLaunchSpeed - (_errorAmount * errorPointsAdj);
                launchSpeed = Mathf.Clamp(launchSpeedAdjCurve.Evaluate(_rawLaunchSpeed),minLaunchSpeed,maxLaunchSpeed);
                debugGetRawLaunchSpeed = maxLaunchSpeed - (_errorAmount * errorPointsAdj);
                debugGetErrorAmount = _errorAmount;
                currSpeed = launchSpeed;
                maxUpSpeed = Mathf.Clamp(upSpeedCurve.Evaluate(_rawLaunchSpeed * launchToUpSpeedRatio), clampMinUpSpeed, clampMaxUpSpeed);
            }
        }
    }
}
