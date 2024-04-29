using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PartyGameGlowing : MonoBehaviour
        {
            [Tooltip("default is 0.9, 7, 0.5")]
            [SerializeField] private bool defaultSetting;
            [Tooltip("Cycle the alpha and not the value of the color. Can be used with [defaultSetting]")]
            [SerializeField] private bool cycleAlpha;
            [Tooltip("Plus this to the V of the color")]
            [SerializeField] private float valuePlusAdj;
            [SerializeField] private float lightCycleSpeed;
            [Tooltip("Amplitude of the sin wave")]
            [SerializeField] private float lightCycleAmp;

            public bool IsGlowing; 
            //For returning the color to turn off
            private Color _initColor;
            private SpriteRenderer _sprRend;
            [Tooltip("Logs the final value")]
            [SerializeField] private bool debugMode;

            private void OnEnable()
            {
                _sprRend = GetComponent<SpriteRenderer>();
                _initColor = _sprRend.color;
                IsGlowing = true;

                if (defaultSetting)
                {
                    valuePlusAdj = 0.9f;
                    lightCycleSpeed = 7;
                    lightCycleAmp = 0.5F;
                }
            }

            void Update()
            {
                if (!IsGlowing)
                {
                    return;
                }
                //Breathing Light
                float h;
                float s;
                float v;
                float _sinValue = (Mathf.Sin(Time.time * lightCycleSpeed) * lightCycleAmp) + valuePlusAdj;

                if (cycleAlpha)
                {
                    _sprRend.color = new Color(_sprRend.color.r, _sprRend.color.g, _sprRend.color.b, _sinValue);
                }
                else
                {
                    Color.RGBToHSV(_sprRend.color, out h, out s, out v);
                    v = _sinValue;
                    _sprRend.color = Color.HSVToRGB(h, s, v);
                }
                if (debugMode) { print(_sinValue);}
            }
            public void StopGlow()
            {
                _sprRend.color = _initColor;
                IsGlowing = false;
            }
            public void StopGlowKeepColor()
            {
                IsGlowing = false;
            }
            public void StartGlow()
            {
                IsGlowing = true;
            }
        }
    }
}
