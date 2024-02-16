using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RitualNight
{
    namespace PartyGames
    {
        [ExecuteInEditMode]
        public class ScrollCameraManager_RB : MonoBehaviour
        {
            [SerializeField] private AnimationCurve cameraSizeCurve;
            [SerializeField] private Camera mainCam;
            [SerializeField] private float cameraInitSize;

            [SerializeField] private AnimationCurve maskSizeCurve;
            [SerializeField] private GameObject maskObject;
            [SerializeField] private float maskInitSize;

            [Header("Anchors")]
            public Transform StartPos;
            public Transform EndPos;

            [Header("Debug")]
            public bool DebugMode;
            // Update is called once per frame
            void OnDrawGizmos()
            {
                if (DebugMode)
                {
                    float _currentPosPercent = Mathf.Clamp01((StartPos.position.y - transform.position.y) / (StartPos.position.y - EndPos.position.y));
                    mainCam.orthographicSize = cameraInitSize + cameraSizeCurve.Evaluate(_currentPosPercent);
                    float _size = maskInitSize + maskSizeCurve.Evaluate(_currentPosPercent);                    
                    maskObject.transform.localScale = new Vector3(_size, _size, _size);
                }
            }
            private void Update()
            {
                float _currentPosPercent = Mathf.Clamp01((StartPos.position.y - transform.position.y) / (StartPos.position.y - EndPos.position.y));
                mainCam.orthographicSize = cameraInitSize + cameraSizeCurve.Evaluate(_currentPosPercent);
                float _size = maskInitSize + maskSizeCurve.Evaluate(_currentPosPercent);
                maskObject.transform.localScale = new Vector3(_size, _size, _size);
            }
        }
    }
}