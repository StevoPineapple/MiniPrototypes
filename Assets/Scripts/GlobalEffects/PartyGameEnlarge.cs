using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        /// <summary>
        /// Enlarge object, assume x and y use the same size.
        /// </summary>
        public class PartyGameEnlarge : MonoBehaviour
        {
            [Tooltip ("Enlarge size is *1.1")]
            [SerializeField] bool defaultSetting;
            [Tooltip("Change a Sprite Renderer somewhere else")]
            [SerializeField] bool separateSprRend;
            [Tooltip("Only used if separated SprRend is true")]
            [SerializeField] SpriteRenderer otherSprRend;
            [SerializeField] private float enlargeRate;
            private Vector3 _originalSize;
            private void OnEnable()
            {
                if (defaultSetting)
                {
                    enlargeRate = 1.1f;
                }
                SetOriginalSize();
            }
            /// <summary>
            /// Set the OG size, enlarge will be this times enlargeRate;
            /// </summary>
            public void SetOriginalSize()
            {
                if (separateSprRend)
                { _originalSize = otherSprRend.gameObject.transform.localScale;}
                else
                { _originalSize = transform.localScale;}
            }
            public void Enlarge()
            {
                if (separateSprRend)
                { otherSprRend.gameObject.transform.localScale = _originalSize * enlargeRate; }
                else
                { transform.localScale = _originalSize*enlargeRate; }
            }
            public void ResetSize()
            {
                if (separateSprRend)
                { otherSprRend.gameObject.transform.localScale = _originalSize; }
                else
                { transform.localScale = _originalSize; }
            }
        }
    }
}
