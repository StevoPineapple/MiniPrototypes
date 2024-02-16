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
            private void Awake()
            {
                _sprRend = transform.GetChild(0).GetComponent<SpriteRenderer>();
            }
            public void Pass(int _direction)
            {
                spinSpeed = spinSpeedMax * -_direction;
            }
            // Update is called once per frame
            void Update()
            {
                if (Mathf.Abs(spinSpeed) > spinSpeedMin)
                {
                    _sprRend.gameObject.transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime,0));
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
        }
    }
}