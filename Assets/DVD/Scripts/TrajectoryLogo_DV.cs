using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace RitualNight
{
    namespace PartyGames
    {
        public class TrajectoryLogo_DV : MonoBehaviour
        {
            [Header ("Components")]
            [SerializeField] private GameObject dotObject;
            private Transform _dotParent;
            private Collider2D _selfCollider;
            private Rigidbody2D _selfBody;

            private int _givenLife;
            private int _selfLife;

            [HideInInspector] public bool CanSetNewLife;

            private float _trajSpeedMulti;
            private Vector2 _selfVelocity;

            public void Awake()
            {   
                _selfCollider = GetComponent<Collider2D>();
                _selfBody = GetComponent<Rigidbody2D>();
                CanSetNewLife = false;
            }

            public void Launch(Transform _tvParent, Transform _initTrans, Transform _dotParent, Vector2 direction, int _life, float _trajSpdMulti )
            {
                this._dotParent = _dotParent;
                _givenLife = _life;

                _trajSpeedMulti = _trajSpdMulti;
                IgnoreCollision(_tvParent);
                SetLaunchAngle(_initTrans, direction);
            }
            public void IgnoreCollision(Transform _tvParent)//use for each because used to be many logos 
            {
                foreach (Transform child in _tvParent)
                {
                    Physics2D.IgnoreCollision(child.GetComponent<Collider2D>(), _selfCollider);
                }
            }
            public void SetLaunchAngle(Transform _initTrans, Vector2 _velo)
            {
                _velo *= _trajSpeedMulti;
                _selfBody.velocity = new Vector2(_velo.x, _velo.y);
            }

            private void Update()
            {
                _selfVelocity = _selfBody.velocity;
            }
            private void OnCollisionEnter2D(Collision2D collision)
            {
                if (collision.gameObject.CompareTag("SideUp_DV"))
                {
                    OnHitSide(collision);
                }
                else if (collision.gameObject.CompareTag("SideDown_DV"))
                {
                    OnHitSide(collision);
                }
                else if (collision.gameObject.CompareTag("SideLeft_DV"))
                {
                    OnHitSide(collision);
                }
                else if (collision.gameObject.CompareTag("SideRight_DV"))
                {
                    OnHitSide(collision);
                }
            }

            private void OnHitSide(Collision2D collision)
            {
                _selfVelocity = Vector2.Reflect(_selfVelocity, -collision.GetContact(0).normal);
                _selfBody.velocity = _selfVelocity;
            }

            public void SetNewLife(Vector3 _initPos, Vector2 _velo)
            {
                _velo *= _trajSpeedMulti;
                CanSetNewLife = false;
                transform.position = _initPos;
                _selfLife = _givenLife;
                _selfBody.velocity = Vector3.zero;
                _selfBody.velocity = new Vector2(_velo.x, _velo.y);
            }
            private void FixedUpdate()
            {
                if (_selfLife > 0)
                {
                    Instantiate(dotObject, transform.position, transform.rotation, _dotParent);
                    _selfLife--;
                }
                else
                {
                    CanSetNewLife = true;
                }
            }
            public void ResetLife()
            {
                _selfLife = 0;
                CanSetNewLife = true;
            }
        }
    }
}