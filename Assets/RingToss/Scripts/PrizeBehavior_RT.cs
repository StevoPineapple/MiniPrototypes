using System.Collections;
using System.Collections.Generic;
using UnityEngine;
        public class PrizeBehavior_RT : MonoBehaviour
        {
            public bool IsModified;
            public bool IsChosen;
            private Vector3 moveDirection;
            public Transform RingTransform;
            private float _ringRadius;
            [SerializeField] private float moveSpeedAdj;
            [SerializeField] private float moveLerp;
            private float moveSpeed;

            [Header("Float Control")]
            private Vector3 _initPos;
            private Vector3 _initScale;
            private Vector3 _initEdgeScale;
            private Vector3 _initMaskScale;

            [SerializeField] private float magnitude;
            [SerializeField] private float randomSinSpeed;

            [Header("Sprite")]
            public SpriteRenderer SprRend;
            public SpriteRenderer EdgeSprRend;
            public SpriteMask SprMask;

            private GameObject SprRendObject;
            private GameObject EdgeSprRendObject;
            private GameObject SprMaskObject;

            public Sprite RedSprite;
            public Sprite SpSprite;
            // Start is called before the first frame update
            void Start()
            {
                SprRendObject = SprRend.gameObject;
                EdgeSprRendObject = EdgeSprRend.gameObject;
                SprMaskObject = SprMask.gameObject;

                _initScale = transform.localScale;

                _initEdgeScale = _initScale * 0.88f;
                _initMaskScale = _initScale * 0.7f;

                _initPos = transform.position;
                randomSinSpeed = Random.Range(0, randomSinSpeed);
            }

            // Update is called once per frame
            void FixedUpdate()
            {
                if (IsChosen)
                {
                    Vector3 _distanceToCenter = (RingTransform.position - transform.position);
                    moveDirection = _distanceToCenter.normalized;
                    transform.Translate(moveDirection * moveSpeed);
                    moveSpeed = Mathf.Lerp(moveSpeed, 0, moveLerp);
                    _initPos = transform.position;
                    if (Vector2.Distance(RingTransform.position, transform.position) > _ringRadius)
                    {
                        float _clampDistance = Mathf.Clamp(_distanceToCenter.magnitude, 0, _ringRadius);
                        _initPos = RingTransform.position + moveDirection * -_clampDistance;
                    }
                }

                float _sinResult = Mathf.Sin(Time.time * randomSinSpeed) * magnitude;
                float _sinResultPos = _sinResult * 0.5f;

                SprRendObject.transform.localScale = _initScale + new Vector3(_sinResult, _sinResult, _sinResult);
                SprRendObject.transform.position = _initPos + new Vector3(0, _sinResultPos, 0);

                EdgeSprRendObject.transform.localScale = (_initEdgeScale) + (new Vector3(_sinResult, _sinResult, _sinResult) * 1.3f);
                EdgeSprRendObject.transform.position = _initPos + new Vector3(0, _sinResultPos, 0);

                SprMaskObject.transform.localScale = _initMaskScale + (new Vector3(_sinResult, _sinResult, _sinResult) * 1.5f);
                //EdgeSprRendObject.transform.localScale = SprMaskObject.transform.localScale * 1.2f;
                //SprMaskObject.transform.position = _initPos + new Vector3(0, _sinResultPos, 0);


                //Movement
                //PSprObj -> sine wave
                //PingPong Random
            }
            public void ChangeToRed()
            {
                SprRend.sprite = RedSprite;
                EdgeSprRend.sprite = RedSprite;
                SprMask.sprite = RedSprite;
                tag = "PrizeRed_RT";
            }
            public void ChangeToSpecial()
            {
                SprRend.sprite = SpSprite;
                EdgeSprRend.sprite = SpSprite;
                SprMask.sprite = SpSprite;
                tag = "PrizeSpecial_RT";
            }
            public void GotChosen(Vector3 direction, float distance, Transform _ringTransform, float _ringRadiusValue)
            {
                SprRend.sortingOrder += 100;
                SprMask.frontSortingOrder += 100;
                SprMask.backSortingOrder += 100;
                IsChosen = true;
                moveDirection = direction;
                moveSpeed = distance * moveSpeedAdj;
                RingTransform = _ringTransform;
                _ringRadius = _ringRadiusValue * _ringTransform.lossyScale.x;
            }

            public void SetSortingOrder(int frontOrder)
            {
                SprRend.sortingOrder = frontOrder;
                SprMask.frontSortingOrder = frontOrder;
                SprMask.backSortingOrder = frontOrder - 1;
            }
        }