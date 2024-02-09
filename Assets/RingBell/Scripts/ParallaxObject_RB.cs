using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RitualNight
{
    namespace PartyGames
    {
        //[ExecuteInEditMode]
        public class ParallaxObject_RB : MonoBehaviour
        {
            public Transform AnchorTrans;
            public float adjustAmount;
            private Vector3 _initPos;
            private Vector3 _anchorInitPos;
            private Vector3 _parentInitPos;
            private bool _isParented;
            //public bool Stationary;

            [Header("Debug")]
            public bool DebugMode;
            private bool _hasStartedDebugMode;
            [Header("Toggle to Set Pos")]
            public bool ViewMode;

            private bool _hasSetNewPos;
            private bool _hasReturned;

            [Header("Set Pos in View Mode")]
            public bool SetPosInView;

            private void Awake()
            {
                if (DebugMode)
                {
                    _hasStartedDebugMode = true;
                }
                _initPos = transform.position;
                _anchorInitPos = AnchorTrans.position;
                _isParented = (transform.parent.tag == "Parent_RB");
                if (_isParented)
                {
                    _parentInitPos = transform.parent.position;
                }
            }
            private void FixedUpdate()
            {
                if (DebugMode)
                {
                    _isParented = (transform.parent.tag == "Parent_RB");
                }
            }
            public void SetViewMode(bool _state)
            {
                ViewMode = _state;
                DebugMode = true;
            }
            public void SetPosInViewMode(bool _value)
            {
                SetPosInView = _value;
            }
            private void OnDrawGizmos()
            {
                if (!DebugMode)
                {
                    _hasStartedDebugMode = false;
                    return;
                }
                else if (!_hasStartedDebugMode)
                {
                    _hasStartedDebugMode = true;
                    EnterDebugMode();
                }
                if (ViewMode)
                {
                    if (SetPosInView) //Init pos
                    {
                        _initPos = transform.position - new Vector3(0, +AnchorTrans.position.y * adjustAmount, 0);
                        if (_isParented)
                        {
                            _parentInitPos = transform.parent.position;
                        }
                    }
                    if (!_hasSetNewPos)
                    {
                        _hasReturned = false;
                        _initPos = transform.position;
                        if (_isParented)
                        {
                            _parentInitPos = transform.parent.position;
                        }
                        _hasSetNewPos = true;
                    }
                    else
                    {
                        ParallaxMovement();
                    }
                }
                else
                {
                    _hasSetNewPos = false;
                    if (!_hasReturned)
                    {
                        transform.position = _initPos;
                        _hasReturned = true;
                    }
                }
            }
            private void ParallaxMovement()
            {
                //if (_isParented)
                //{
                //    transform.position = new Vector3(transform.position.x, _initPos.y + (AnchorTrans.position.y - _anchorInitPos.y) * adjustAmount - (transform.parent.position.y - _parentInitPos.y), 0);
                //}
                //else
                //{
                    transform.position = new Vector3(transform.position.x, _initPos.y + (AnchorTrans.position.y - _anchorInitPos.y) * adjustAmount, 0);
                //}
            }
            private void Update()
            {
                ParallaxMovement();
            }
            private void EnterDebugMode()
            {
                _initPos = transform.position;
                if (_isParented)
                {
                    _parentInitPos = transform.parent.position;
                }
            }

        }
    }
}
