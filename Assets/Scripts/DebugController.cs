using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RitualNight.PartyGames;

namespace RitualNight
{
    namespace PartyGames
    {
        public class DebugController : MonoBehaviour
        {
            private int _initFrameRate;
            private int _initStableFrameRate;
            private bool _isUnstableFPS;
            public int UnstableFPS;
            void Awake()
            {
                _initFrameRate = Application.targetFrameRate;
            }

            // Update is called once per frame
            void Update()
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Application.targetFrameRate = 10;
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Application.targetFrameRate = 30;
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Application.targetFrameRate = 60;
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Application.targetFrameRate = 200;
                }
                if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    if (_isUnstableFPS) 
                    {
                        Application.targetFrameRate = 60;
                        _isUnstableFPS = false; 
                    }
                    else 
                    { 
                        _isUnstableFPS = true;
                        _initStableFrameRate = Application.targetFrameRate;
        }
                }
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    Application.targetFrameRate = _initFrameRate;
                }
                if (_isUnstableFPS)
                {
                    UnstableFPS = _initStableFrameRate + Random.Range(-15, 15);
                    Application.targetFrameRate = UnstableFPS;
                }
            }
        }
    }
}
