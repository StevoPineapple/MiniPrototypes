using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PartyGameShaking : MonoBehaviour
        {
            [Tooltip ("Default is shakeSelf True, shakeLocal False, maxShake 0.15, minShake 0.01, reduceLerp 0.3 ")]
            public bool DefaultValue;
            [Tooltip("Shake LocalPosition?")]
            [SerializeField] private bool isShakeLocal;
            [Tooltip("Shake self transform? If not provide transform for shakeTrans")]
            [SerializeField] private bool isShakeSelf;
            [Tooltip("Unsed if isShakeSelf is false")]
            [SerializeField] private Transform shakeTrans;
            [Tooltip("Amount of lerp used to reduce the shake amount")]
            [SerializeField] private float shakeReduceLerp;
            [SerializeField] private float maxShakeAmount;
            [SerializeField] private float minShakeAmount;
            [SerializeField] private bool setInitPosOnAwake;
            private bool _isShaking;
            private Vector3 _initPos;
            private float _currentShakeAmount;
            [Header ("Debug")]
            public bool DebugMode; 
            void Awake()
            {
                if (DefaultValue)
                {
                    isShakeSelf = true;
                    isShakeLocal = false;
                    maxShakeAmount = 0.15f;
                    minShakeAmount = 0.01f;
                    shakeReduceLerp = 0.3f;
                    setInitPosOnAwake = true;
                }
                if (setInitPosOnAwake)
                {
                    SetInitPos();
                }
                _isShaking = false;
            }
#if UNITY_EDITOR
            private void OnDrawGizmos()
            {
                if (DebugMode)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(transform.position, 0.1f);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(_initPos, 0.1f);
                }
            }
#endif
            void FixedUpdate()
            {
                if (_isShaking)
                {
                    float _randomX = Random.Range(-_currentShakeAmount,_currentShakeAmount);
                    float _randomY = Random.Range(-_currentShakeAmount, _currentShakeAmount);
                    if (isShakeSelf)
                    {
                        if (isShakeLocal)
                            transform.localPosition = _initPos + new Vector3(_randomX, _randomY, 0);
                        else
                            transform.position = _initPos + new Vector3(_randomX, _randomY, 0);
                    }
                    else
                    {
                        if (isShakeLocal)
                            shakeTrans.localPosition = _initPos + new Vector3(_randomX, _randomY, 0);
                        else
                            shakeTrans.position = _initPos + new Vector3(_randomX, _randomY, 0);
                    }
                    _currentShakeAmount = Mathf.Lerp(_currentShakeAmount, 0, shakeReduceLerp);
                    if (_currentShakeAmount <= minShakeAmount)
                    {
                        ShakeStop();
                    }
                }
            }
            [Tooltip ("Set the anchor position for object to self position")]
            public void SetInitPos()
            {
                if(isShakeSelf)
                    _initPos = transform.position;
                else
                    _initPos = shakeTrans.position;
            }
            [Tooltip("Set the anchor position for object")]
            public void SetInitPos(Vector3 _pos)
            {
                if (isShakeSelf)
                    _initPos = _pos;
                else
                    _initPos = _pos;
            }
            [Tooltip ("Start shaking")]
            public void ShakeStart()
            {
                _isShaking = true;
                _currentShakeAmount = maxShakeAmount;
            }
            [Tooltip("Start shaking but only stop manually")]
            public void ShakeStop()
            {
                _isShaking = false;
                _currentShakeAmount = 0;
                ResetPosition();
            }
            [Tooltip("Reset to anchor. Used internally, only call for explicit use.")]
            public void ResetPosition()
            {
                if (isShakeSelf)
                {
                    if (isShakeLocal)
                        transform.localPosition = _initPos;
                    else
                        transform.position = _initPos;
                }
                else
                {
                    if (isShakeLocal)
                        shakeTrans.localPosition = _initPos;
                    else
                        shakeTrans.position = _initPos;
                }
            }
        }
    }
}
