using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RitualNight
{
    namespace PartyGames
    {
        [ExecuteInEditMode]
        public class ParallaxManager_RB : MonoBehaviour
        {
            public ScrollCameraManager_RB CamManager;
            public bool DebugMode;
            public bool AllViewMode;
            public bool AllSetPosInView;
            private bool _hasReturnToCamStart;

            [Header("Slidey")]
            public bool IsLaunched;
            [SerializeField] private GameObject ballObject;
            [SerializeField] private float gravity;
            [SerializeField] private AnimationCurve gravityCurve;
            [SerializeField] private float rawSpeed;
            [SerializeField] private float maxUpSpeed;
            [SerializeField] private float initUpSpeed;

            [Header ("AutoMotion")]
            public bool AutoMotion;
            public float Amp;
            public float Frequency;
            public float Offset;

            private bool _isAutoMotionInit;
            private Vector3 _autoMotionInitPos;

            //Debug
            private float debugSpeed;
            private float debugAcc = 0.1f;
            private float debugSpeedMax = 2;

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
            }
            private void OnDrawGizmos()
            {
                if (!DebugMode)
                { 
                    return;
                }

                if (Input.GetKey(KeyCode.LeftBracket))
                {
                    print("pressed");
                    debugSpeed = Mathf.Clamp(debugSpeed + debugAcc, 0, debugSpeedMax);
                    transform.position += new Vector3(0, debugSpeed, 0);
                }
                else if (Input.GetKey(KeyCode.RightBracket))
                {
                    debugSpeed = Mathf.Clamp(debugSpeed + debugAcc, 0, debugSpeedMax);
                    transform.position -= new Vector3(0, debugSpeed, 0);
                }
                else
                {
                    debugSpeed = 0;
                }

                if (AllViewMode)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.yellow;
                    style.fontSize = 30;

                    Handles.Label(transform.position, "VIEW MODE", style);

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
                }
            }//Debug mode
#endif
            private void FixedUpdate()
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    transform.position = CamManager.StartPos.position;
                    IsLaunched = false;
                    rawSpeed = 0;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                   
                }

                if (IsLaunched)
                {
                    rawSpeed -= gravityCurve.Evaluate(rawSpeed);
                    if (rawSpeed <= 0)
                    {
                        //SetHeight;
                    }
                    transform.position += new Vector3(0, -Mathf.Clamp(rawSpeed, -999, maxUpSpeed), 0);
                }
            }

            private void Start()
            {
                transform.position = CamManager.StartPos.position;
            }
            public void Launch()
            {
                IsLaunched = true;
                rawSpeed = initUpSpeed;
                maxUpSpeed = Mathf.Clamp(initUpSpeed / 10, 0.5f, 1.4f);
            }
        }
    }
}
