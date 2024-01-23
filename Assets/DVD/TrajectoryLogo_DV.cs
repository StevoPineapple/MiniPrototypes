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
            // Start is called before the first frame update
            private Collider2D _selfCollider;
            private Rigidbody2D _selfBody;
            private Vector3 _initAimingPos;
            [SerializeField] private int _givenLife;
            [SerializeField] private int selfLife;
            public bool CanSetNewLife;
            private int _dropTick;
            [SerializeField] private int dropTickMax;
            [SerializeField] private GameObject dotObject;
            [SerializeField] private Transform dotParent;
            public Transform detectParent;

            public void Awake()
            {
                detectParent = GameObject.Find("DetectorObjects").transform;
                _selfCollider = GetComponent<Collider2D>();
                _selfBody = GetComponent<Rigidbody2D>();
                CanSetNewLife = false;
                _dropTick = dropTickMax;
            }

            public void Launch(Transform _tvParent, Transform _initTrans, Transform _dotParent, Vector2 direction, int _life )
            {
                dotParent = _dotParent;
                _givenLife = _life;
                IgnoreCollision(_tvParent);
                SetLaunchAngle(_initTrans, direction);
            }
            public void IgnoreCollision(Transform _tvParent)
            {
                foreach (Transform child in _tvParent)
                {
                    Physics2D.IgnoreCollision(child.GetComponent<Collider2D>(), _selfCollider);
                }
            }
            public void SetLaunchAngle(Transform _initTrans, Vector2 _velo)
            {
                _velo *= 6;
                _initAimingPos = _initTrans.position;
                _selfBody.velocity = new Vector2(_velo.x, _velo.y);
            }

            public void SetNewLife(Vector3 _initPos, Vector2 _velo)
            {
                _velo *= 6;
                CanSetNewLife = false;
                transform.position = _initPos;
                selfLife = _givenLife;
                _selfBody.velocity = Vector3.zero;
                _selfBody.velocity = new Vector2(_velo.x, _velo.y);
            }
            private void FixedUpdate()
            {
                if (selfLife > 0)
                {
                    selfLife--;
                }
                else
                {
                    CanSetNewLife = true;
                }

                if (_dropTick > 0)
                {
                    _dropTick--;
                }
                else
                {
                    _dropTick = dropTickMax;
                    Instantiate(dotObject, transform.position, transform.rotation, dotParent);
                }

            }
            public void ResetLife()
            {
                selfLife = 0;
                CanSetNewLife = true;
            }
            private void OnCollisionEnter2D(Collision2D collision)
            {
                if (collision.gameObject.CompareTag(""))
                {
                    
                }
            }
        }
    }
}