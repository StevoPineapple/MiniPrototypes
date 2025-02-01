using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class StarFormation_AS : MonoBehaviour
        {
            public SpriteRenderer ArtSprRend;
            public Transform StarHolder;
            private void OnEnable()
            {
                foreach (Transform child in StarHolder)
                {
                    child.gameObject.GetComponent<StarBehavior_AS>().IsGood = true;
                }
            }
        }
    }
}