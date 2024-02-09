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
                    mainCam.orthographicSize = cameraInitSize + cameraSizeCurve.Evaluate(Mathf.Clamp01((StartPos.position.y - transform.position.y) / (StartPos.position.y - EndPos.position.y)));
                }
            }
            private void Update()
            {
                mainCam.orthographicSize = cameraInitSize + cameraSizeCurve.Evaluate(Mathf.Clamp01((StartPos.position.y - transform.position.y) / (StartPos.position.y - EndPos.position.y)));
            }
        }
    }
}