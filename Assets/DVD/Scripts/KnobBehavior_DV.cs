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
            public int CornerIndex;
            [SerializeField] TvController_DV tvController;
            private Vector3 _initPos;
            [SerializeField] private float maxStretchDist;

            public SpriteRenderer KnobSprite;
            public SpriteRenderer BGSprite;
            private Color _altBGColor = Color.yellow;
            private bool _isShowAltColor;
            public bool IsActive;
            [SerializeField] int _sideFlashTickMax;
            int _sideFlashTick;
            public bool IsStopped;
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
            public void ResetAltColor()
            {
                _altBGColor = Color.yellow;
            }
            public void TurnRed()
            {
                KnobSprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                BGSprite.color = new Color(0.5f, 0f, 0f, 1f);
                _altBGColor = Color.red;
                IsActive = false;
            }
            public void TurnYellow()
            {
                KnobSprite.color = Color.white;
                BGSprite.color = Color.yellow;
                _altBGColor = Color.yellow;
                IsActive = true;
            }
            public void BlendColor(float _amount)
            {
                KnobSprite.color = Color.white;
                _altBGColor = new Color(1, _amount, 0, 1);
            }
            public void OnGrab()
            {
                if (!IsActive)
                {
                    return;
                }
                if (CornerIndex == 1 || CornerIndex == 2)
                {
                    transform.Rotate(Vector3.forward, -90f);
                }
                else
                {
                    transform.Rotate(Vector3.forward, 90f);
                }
                IsGrabing = true;
                tvController.KnobGrabbed(CornerIndex);
            }
            public void OnRelease()
            {
                transform.rotation = Quaternion.identity;
                IsGrabing = false;
                tvController.KnobReleased();
            }
            public Vector3 ClampPosition()
            {
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, _initPos.x - maxStretchDist, _initPos.x + maxStretchDist),
                    Mathf.Clamp(transform.position.y, _initPos.y - maxStretchDist, _initPos.y + maxStretchDist), 0);
                return transform.position;
            }
            public Vector3 ClampOtherPosition(Vector3 _pos)
            {
                Vector3 _returnPos = new Vector3(
                    Mathf.Clamp(_pos.x, _initPos.x - maxStretchDist, _initPos.x + maxStretchDist),
                    Mathf.Clamp(_pos.y, _initPos.y - maxStretchDist, _initPos.y + maxStretchDist), 0);
                return _returnPos;
            }
            private void FixedUpdate()
            {
                if (IsStopped)
                {
                    return;
                }
                if (IsActive)
                {
                    if (_isShowAltColor)
                    {
                        _sideFlashTick--;
                        if (_sideFlashTick <= 0)
                        {
                            _isShowAltColor = !_isShowAltColor;
                            _sideFlashTick = _sideFlashTickMax;
                            return;
                        }
                        BGSprite.color = _altBGColor;
                    }
                    else
                    {
                        _sideFlashTick--;
                        if (_sideFlashTick <= 0)
                        {
                            _isShowAltColor = !_isShowAltColor;
                            _sideFlashTick = _sideFlashTickMax;
                            return;
                        }
                        BGSprite.color = Color.yellow;
                    }
                }
            }
        }
    }
}