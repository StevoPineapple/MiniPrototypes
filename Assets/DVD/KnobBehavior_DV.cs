using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class KnobBehavior_DV : MonoBehaviour
        {
            public bool IsGrabing;
            [SerializeField] private int cornerIndex;
            [SerializeField] TvController_DV tvController;
            private Vector3 _initPos;
            [SerializeField] private float maxStretchDist;
            // Start is called before the first frame update
            public void SetInitPos()
            {
                _initPos = transform.position;
            }
            public void ReturnToInitPos()
            {
                transform.position = _initPos;
            }

            // Update is called once per frame
            void Update()
            {

            }
            public void OnGrab()
            {
                IsGrabing = true;
                tvController.KnobGrabbed(cornerIndex);
            }
            public void OnRelease()
            {
                IsGrabing = false;
                tvController.ExitGrab();
            }
            public Vector3 ClampPosition()
            {
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, _initPos.x - maxStretchDist, _initPos.x + maxStretchDist),
                    Mathf.Clamp(transform.position.y, _initPos.y - maxStretchDist, _initPos.y + maxStretchDist), 0);
                return transform.position;

            }
        }
    }
}