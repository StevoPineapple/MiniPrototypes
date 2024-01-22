using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class ObjectJumpy_BL : MonoBehaviour
        {
            [SerializeField] private AnimationCurve jumpCurve;
            [SerializeField] private float _jumpMultiplier;
            [SerializeField] private float _jumpIncrease;
            private float _currCurvePosition;
            private bool _isJumping;
            private Vector3 initScale; //assume x and y is same

            private void OnEnable()
            {
                initScale = transform.localScale;
                if (_jumpMultiplier == 0)
                {
                    _jumpMultiplier = 1;
                }
            }
            void FixedUpdate()
            {
                if (_isJumping)
                {
                    float _scale = initScale.x + jumpCurve.Evaluate(_currCurvePosition)*_jumpMultiplier;
                    _currCurvePosition = _currCurvePosition + _jumpIncrease;
                    transform.localScale = new Vector3(_scale, _scale, _scale);
                    if (_currCurvePosition > 1)
                    {
                        _isJumping = false;
                    }
                }
            }

            // Update is called once per frame
            public void Jump(float speed, float multiplier)
            {
                _jumpIncrease = speed;
                _jumpMultiplier = multiplier;
                Jump();
            }
            public void Jump()
            {
                _currCurvePosition = 0;
                _isJumping = true;
            }
        }
    }
}