using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class Background_AS : MonoBehaviour
        {
            public PolygonCollider2D BGCollider;
            public SpriteRenderer BGSprRend;
            public Sprite photoBG;
            public SpriteRenderer ArtBGRend;
            [Header("ConstellationParents")]
            [SerializeField] private Transform[] constellationParentArr;
            //Add them to on enable at the bottom
            [SerializeField] private Transform cthulhuParent;
            [SerializeField] private Transform snakeParent;
            [SerializeField] private Transform cakeParent;
            [SerializeField] private Transform sofaParent;
            private void OnEnable()
            {
                foreach (Transform _parent in constellationParentArr)
                {
                    DeactiveAllChildren( _parent );
                    if (_parent.localScale.x != 1 || _parent.localScale.y != 1)
                    {
                        Debug.LogError("Constellation Parent not sclaed to 1, fix in prefab");
                    }
                }
            }
            public Transform FindStarFormation(ConstellationManager_AS.ConstellationName _name)
            {
                switch (_name)
                {
                    case (ConstellationManager_AS.ConstellationName.Cthulhu):
                        {
                            return SelectRandomChild(cthulhuParent);
                        }
                    case (ConstellationManager_AS.ConstellationName.Snake):
                        {
                            return SelectRandomChild(snakeParent);
                        }
                    case (ConstellationManager_AS.ConstellationName.Cake):
                        {
                            return SelectRandomChild(cakeParent);
                        }
                    case (ConstellationManager_AS.ConstellationName.Sofa):
                        {
                            return SelectRandomChild(sofaParent);
                        }

                }
                Debug.LogError("Constellation not found");
                return null;
            }
            private Transform SelectRandomChild(Transform _trans)
            {
                Transform _chosenTrans = _trans.GetChild(Random.Range(0, _trans.childCount));
                _chosenTrans.gameObject.SetActive(true);
                print(_chosenTrans);
                return _chosenTrans;
            }
            private void DeactiveAllChildren(Transform _trans)
            {
                foreach(Transform _child in _trans)
                {
                    _child.gameObject.SetActive(false);    
                }
            }

        }
    }
}