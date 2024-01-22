using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class LogoBehavior_DV : MonoBehaviour
        {
            public PlayerController_DV PlayerController;
            public float speed;
            private Rigidbody2D _selfBody;
            private Collider2D _selfCollider;
            [SerializeField] private Vector3 currentAngle;
            [SerializeField] private bool preExist;
            private bool _isAiming;
            private bool _hasLaunched;
            private bool _isInSlowMode;

            [SerializeField] private GameObject copyPrefab;
            [SerializeField] private Transform tvParent;
            [SerializeField] private Transform copyParent;
            [SerializeField] private Transform dotParent;
            private TrajectoryLogo_DV _copyObject;
            //[SerializeField] private bool
            private void Awake()
            {
                _selfBody = GetComponent<Rigidbody2D>();
                _selfCollider = GetComponent<Collider2D>();

                if (preExist)
                {
                    _selfBody.AddForce(new Vector2(speed, speed));
                    //tvParent = _tvParent;
                    //copyParent = _copyParent;
                    //_isReady = true;

                    _copyObject = Instantiate(copyPrefab, transform.position, transform.rotation, copyParent).GetComponent<TrajectoryLogo_DV>();
                    _copyObject.gameObject.SetActive(false);
                }
            }
            public TrajectoryLogo_DV CreateLaunchCopy(Transform _tvParent, Transform _copyParent, Transform _dotParent, Vector3 _direction, int _life)
            {
                dotParent = _dotParent;
                tvParent = _tvParent;
                copyParent = _copyParent;
                _isAiming = true;

                _copyObject = Instantiate(copyPrefab, transform.position, transform.rotation, copyParent).GetComponent<TrajectoryLogo_DV>();
                _copyObject.Launch(_tvParent,transform, dotParent,_direction, _life);
                return _copyObject;
            }
            public void SetAngle(Vector3 direction)
            {
                if (_isAiming&&!_hasLaunched)
                {
                    currentAngle = direction;
                }
            }
            public void Launch()
            { 
                if (!_hasLaunched)
                {
                    _hasLaunched = true;
                    _isAiming = false;
                    _selfBody.velocity = new Vector2(currentAngle.x, currentAngle.y)*0.1f;
                }
            }
            private void Update()
            {
                if (PlayerController.IsSlow && !_isInSlowMode)
                {
                    _selfBody.velocity *= 0.1f;
                    _isInSlowMode = true;


                }
                else if (!PlayerController.IsSlow && _isInSlowMode)
                {
                    _selfBody.velocity *= 10f;
                    _isInSlowMode = false;
                    _copyObject.gameObject.SetActive(false);
                }
                if (PlayerController.IsSlow)
                {
                    if (_copyObject.CanSetNewLife)
                    {
                        _copyObject.SetNewLife(transform.position, _selfBody.velocity*10);
                    }
                }
            }
            public void ResetCopy()
            {
                _copyObject.SetNewLife(transform.position, _selfBody.velocity*10);
            }
            private void OnCollisionEnter2D(Collision2D collision)
            {
                if (collision.gameObject.CompareTag("Corner_DV"))
                {
                    Destroy(gameObject);
                }
                else if (collision.gameObject.CompareTag("Trajectory_DV"))
                {
                    Physics2D.IgnoreCollision(collision.collider, _selfCollider);
                }
            }
            public void LaunchTrajectoryCopy()
            {
                if (!_isAiming)
                {
                    _copyObject.gameObject.SetActive(true);
                    _copyObject.ResetLife();
                    _copyObject.Launch(tvParent, transform, dotParent, _selfBody.velocity, 10);
                }
            }
        }
    }
}