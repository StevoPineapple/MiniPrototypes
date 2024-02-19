using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class Triangle_RB : MonoBehaviour
        {
            private SpriteRenderer _sprRend;
            [SerializeField] private float spinSpeedMax;
            [SerializeField] private float spinSpeed;
            [SerializeField] private float spinSpeedDecreaseLerp;
            [SerializeField] private float spinSpeedMin;
            private bool _isPuttingDown;
            private float _puttingDownCurveTime;
            [SerializeField] private float _puttingDownCurveTimeIncrease;
            [SerializeField] private AnimationCurve putDownCurve;
            private void Awake()
            {
                _sprRend = transform.GetChild(0).GetComponent<SpriteRenderer>();
            }
            public void Pass(int _direction)
            {
                spinSpeed = spinSpeedMax * -_direction;
            }
            // Update is called once per frame
            private void FixedUpdate()
            {
                if (_isPuttingDown)
                {
                    _sprRend.gameObject.transform.localPosition = new Vector3(0, putDownCurve.Evaluate(_puttingDownCurveTime), 0);
                    _puttingDownCurveTime += _puttingDownCurveTimeIncrease;
                    if (_puttingDownCurveTime >= 1)
                    {
                        _isPuttingDown = false;
                    }
                }
            }
            void Update()
            {
                if (Mathf.Abs(spinSpeed) > spinSpeedMin)
                {
                    _sprRend.gameObject.transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
                    spinSpeed = Mathf.Lerp(spinSpeed, 0, spinSpeedDecreaseLerp);
                }
                else
                {
                    _sprRend.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            public void BlendColor(Color _col)
            {
                _sprRend.color = _col;
            }
            public void ResetGame()
            {
                _isPuttingDown = false;
                _puttingDownCurveTime = 0;
                _sprRend.gameObject.transform.localPosition = Vector3.zero;
                _sprRend.gameObject.transform.localRotation = Quaternion.identity;
                _sprRend.color = Color.white;
            }
            public void PuttingDown()
            {
                _isPuttingDown = true;
            }
        }
    }
}