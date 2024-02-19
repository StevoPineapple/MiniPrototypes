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
            [SerializeField] private float adjustAmount;

            private Vector3 _initPos;
            private Vector3 _anchorInitPos;
            private Vector3 _parentInitPos;
            private SpriteRenderer _sprRend;
            private bool _isParented;
            //public bool Stationary;

            [Header("Render by Range")]
            public bool IsActiveByRange;
            [SerializeField] private float activeRange;

            [Header("Movement")]
            public bool IsMoving;
            [SerializeField] private float moveSpeed;

            [Header("Skew")]
            public bool IsSkewing;
            [SerializeField] private float startSkew;
            [SerializeField] private Transform startSkewPoint;
            [SerializeField] private float endSkew;
            [SerializeField] private Transform endSkewPoint;
            private float _skewPosRange;

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
                _sprRend = GetComponent<SpriteRenderer>();

                if (_isParented)
                {
                    _parentInitPos = transform.parent.position;
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
            public void DiscardInitPos()
            {
                _hasReturned = true;
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
                _isParented = (transform.parent.tag == "Parent_RB");
                if (ViewMode)
                {
                    if (IsSkewing)
                    {
                        _skewPosRange = (endSkewPoint.position.y - startSkewPoint.position.y) + 0.0001f;
                    }
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
                transform.position = new Vector3(transform.position.x, _initPos.y + (AnchorTrans.position.y - _anchorInitPos.y) * adjustAmount, 0);
                if (IsSkewing && _skewPosRange != 0)
                {
                    float _skewAmount = Mathf.Lerp(startSkew, endSkew, Mathf.Clamp((AnchorTrans.position.y - startSkewPoint.position.y), _skewPosRange, 0) / _skewPosRange);
                    transform.localScale = new Vector3(transform.localScale.x, _skewAmount, transform.localScale.z);
                }
            }
            private void ParallaxMovement(float _posY)
            {
                transform.position = new Vector3(transform.position.x, _posY, 0);
                if (IsSkewing && _skewPosRange != 0)
                {
                    float _skewAmount = Mathf.Lerp(startSkew, endSkew, Mathf.Clamp((AnchorTrans.position.y - startSkewPoint.position.y), _skewPosRange, 0) / _skewPosRange);
                    transform.localScale = new Vector3(transform.localScale.x, _skewAmount, transform.localScale.z);
                }
            }
            private void Update()
            {
                if (IsActiveByRange)
                {
                    float _posY = _initPos.y + (AnchorTrans.position.y - _anchorInitPos.y) * adjustAmount;
                    ParallaxMovement(_posY);
                    if (Mathf.Abs(_posY) >= activeRange)
                    {
                        _sprRend.enabled = false;
                        return;
                    }
                    _sprRend.enabled = true;
                    if (IsMoving)
                    {
                        transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
                    }
                }
                else
                {
                    ParallaxMovement();
                }
            }
            private void OnDisable()
            {
                transform.position = _initPos;
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
