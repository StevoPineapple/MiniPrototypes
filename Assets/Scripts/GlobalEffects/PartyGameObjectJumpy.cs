using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PartyGameObjectJumpy : MonoBehaviour
        {
            [SerializeField] private bool defaultSetting;
            [SerializeField] private AnimationCurve jumpCurve;
            [SerializeField] private float _jumpMultiplier;
            [SerializeField] private float _jumpIncrease;
            
            private float _currCurvePosition;
            private bool _isJumping;
            private Vector3 initScale; //assume x and y is same

            private void OnEnable()
            {
                initScale = transform.localScale;
                if (defaultSetting)
                {
                    _jumpMultiplier = 1;
                    _jumpIncrease = 0.12f;

                    //clear curve
                    int keyLength = jumpCurve.keys.Length;
                    if (keyLength > 0)
                    {
                        print(keyLength);
                        print(jumpCurve.keys);
                        for (int i = 0; i < keyLength; i++)
                        {
                            jumpCurve.RemoveKey(0);
                        }
                    }
                    
                    //add keys
                    jumpCurve.AddKey(0, 0);
                    jumpCurve.AddKey(0.5f, 0.5f);
                    jumpCurve.AddKey(1, 0);
                    float value = 2;//go tohell
                    jumpCurve.keys[0].outTangent = value;
                    print("value "+jumpCurve.keys[0].outTangent);
                    jumpCurve.keys[2].inTangent = 2;
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
                        float _finalScale = initScale.x+jumpCurve.keys[jumpCurve.length - 1].value;
                        transform.localScale = new Vector3(_finalScale, _finalScale, _finalScale);
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