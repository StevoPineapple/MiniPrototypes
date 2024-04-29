using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class CardBehavior_CRD : MonoBehaviour
        {
            [SerializeField] private Sprite backSpr;
            [HideInInspector] public Sprite FaceSpr;
            private SpriteRenderer _sprRend;
            private Animator _anim;
            [SerializeField] private GameObject selectEdge;

            private void Awake()
            {
                _sprRend = GetComponent<SpriteRenderer>();
                _anim = GetComponent<Animator>();
            }
            public void SetCardBack(Sprite _backSpr)
            {
                backSpr = _backSpr;
                _sprRend.sprite = _backSpr;
            }
            public void ChangeToFaceSprite()//called in animation
            {
                _sprRend.sprite = FaceSpr;
            }
            public void ChangeToBackSprite()//called in animation
            {
                _sprRend.sprite = backSpr;
            }
            public void FlipToBack()
            {
                _anim.Play("CardFlipToBack");
            }
            public void FlipToFront()
            {
                _anim.Play("CardFlipToFront");
            }
            public void SetEdgeVisible(bool _isVisible)
            {
                selectEdge.SetActive(_isVisible);
            }
            public void DisableSelf()
            {
                tag = "Untagged";
                enabled = false;
            }
            public void EnableSelf()
            {
                tag = "Card_CRD";
            }
        }
    }
}