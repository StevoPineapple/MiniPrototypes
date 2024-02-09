using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class LogoBehavior_DV : MonoBehaviour
        {
            [Header("Components")]
            public PlayerController_DV PlayerController;
            private GameObject _copyPrefab;
            private TrajectoryLogo_DV _copyObject;
            private TvController_DV tvController;
            private Transform _tvParent;
            private Transform _copyParent;
            private Transform _dotParent;
            private Rigidbody2D _selfBody;
            private BoxCollider2D _selfCollider;
            private SpriteRenderer _sprRend;

            [Header("Tuning")]
            [SerializeField] private float slowRate;
            [SerializeField] private float midSlowRate;

            [SerializeField] private Vector3 currentAngle; //useful but can optimize, public to debug
            private bool _isStopped;
            private bool _hasLaunched;

            private int trajectoryLife;
            private float trajectorySpeedMulti;
            private Vector2 _selfVelocity;

            public int _currHitSide; //public to debug
            public int ShortestVertex;

            [Header("Ghost")]
            [SerializeField] private GameObject GhostPrefab;
            private Transform _ghostParent;
            public float ghostDelay = 0.1f;   // Delay between ghost images in seconds

            [Header("Debug")]
            public bool DebugMode;
            //[SerializeField] private bool
            private void Awake()
            {
                _selfBody = GetComponent<Rigidbody2D>();
                _selfCollider = GetComponent<BoxCollider2D>();
                _sprRend = GetComponent<SpriteRenderer>();
            }
            private void Start()
            {
                StartCoroutine(GhostEffect());
            }

            //copy created here
            public TrajectoryLogo_DV CreateLaunchCopy(Vector3 _direction)
            {
                _copyObject = Instantiate(_copyPrefab, transform.position, transform.rotation, _copyParent).GetComponent<TrajectoryLogo_DV>();

                _copyObject.Launch(_tvParent, transform, _dotParent, _direction, trajectoryLife, trajectorySpeedMulti);
                return _copyObject;
            }

            IEnumerator GhostEffect()
            {
                while (true)
                {
                    for (int i = 0; i < tvController.BounceCount; i++)
                    {
                        yield return new WaitForSeconds(ghostDelay);

                        GameObject _ghost = Instantiate(GhostPrefab, transform.position, transform.rotation, _ghostParent);
                        _ghost.GetComponent<SpriteRenderer>().color = new Color(_sprRend.color.r, _sprRend.color.g, _sprRend.color.b,0.5f);
                        Destroy(_ghost, ghostDelay * (tvController.BounceCount + 1)); // Destroy ghosts after a certain time
                    }
                    yield return new WaitForFixedUpdate();
                }
            }

            public void SetAngle(Vector3 direction)
            {
                if (!_hasLaunched)
                {
                    currentAngle = direction;
                }
            }
            public void Launch()
            {
                if (!_hasLaunched)
                {
                    _hasLaunched = true;
                    _selfBody.velocity = new Vector2(currentAngle.x, currentAngle.y);//*0.1f;
                    _selfVelocity = _selfBody.velocity;
                }
            }
            private void Update()
            {
                //Control speed
                if (!_isStopped)
                {
                    if (PlayerController.IsSlow)
                    {
                        _selfBody.velocity = _selfVelocity * slowRate;
                    }
                    else if (PlayerController.IsMidSlow)
                    {
                        _selfBody.velocity = _selfVelocity * midSlowRate;
                    }
                    else
                    {
                        _selfBody.velocity = _selfVelocity;
                    }
                }
                if (PlayerController.IsGrabbing)
                {
                    int _releaseSide = CheckNearEdge();
                    if (_releaseSide!=-1)
                    {
                        //PlayerController.ForceRelease(_releaseSide);
                    }
                }
                else
                {
                    //Any better place to put this?
                    _copyObject.gameObject.SetActive(false);
                }
            }

            [SerializeField] private float farHDist;
            [SerializeField] private float minHDist;
            [SerializeField] private float farVDist;
            [SerializeField] private float minVDist;

            private int CheckNearEdge()
            {
                Vector2[] _sideMidArr = new Vector2[4];

                _sideMidArr[0] = new Vector2(transform.position.x, _selfCollider.bounds.min.y);
                _sideMidArr[1] = new Vector2(_selfCollider.bounds.min.x, transform.position.y);
                _sideMidArr[2] = new Vector2(transform.position.x, _selfCollider.bounds.max.y);
                _sideMidArr[3] = new Vector2(_selfCollider.bounds.max.x, transform.position.y);
                int _ignoreSide = (tvController.BounceSide + 2) % 4;
                int _releaseSide = -1;
                for (int i = 0; i < 4; i++)
                {
                    if (i != _ignoreSide)
                    {
                        if (i == 0)
                        {
                            _releaseSide = CheckNearSideRaycast(_sideMidArr[i], Vector2.down, minVDist, farVDist);
                        }
                        else if (i == 1)
                        {
                            _releaseSide = CheckNearSideRaycast(_sideMidArr[i], Vector2.left, minHDist, farHDist);
                        }
                        else if (i == 2)
                        {
                            _releaseSide = CheckNearSideRaycast(_sideMidArr[i], Vector2.up, minVDist, farVDist);
                        }
                        else if (i == 3)
                        {
                            _releaseSide = CheckNearSideRaycast(_sideMidArr[i], Vector2.right, minHDist, farHDist);
                        }
                    }
                    if (_releaseSide != -1)
                    {
                        return _releaseSide;
                    }
                }
                return -1;
            }
            private int CheckNearSideRaycast(Vector2 sideMid, Vector2 _direction, float _minDist, float _farDist)
            {
                RaycastHit2D[] _hitAll = Physics2D.RaycastAll(sideMid, _direction);
                foreach (RaycastHit2D rayhit in _hitAll)
                {
                    if (rayhit.collider != null)
                    {
                        int _currCheckSide = CheckSides(rayhit);
                        if (_currCheckSide != -1)
                        {
                            Debug.DrawLine(sideMid, rayhit.point, Color.blue, 0.1f);
                            float _dist = Vector2.Distance(sideMid, rayhit.point);

                            if (_dist <= _minDist)
                            {
                                Debug.DrawLine(sideMid, rayhit.point, Color.yellow, 1);
                                return _currCheckSide;
                            }
                            else if (_dist < _farDist)
                            {
                                //-0.2 so the full red color stays longer
                                Debug.DrawLine(sideMid, rayhit.point, Color.white, 0.3f);
                                tvController.BlendSideColor(_currCheckSide, Mathf.Clamp01((_dist - minHDist) / (farHDist - minHDist) - 0.2f));
                                return -1;
                            }
                            else
                            {
                                tvController.BlendSideColor(_currCheckSide, 1);
                            }
                        }
                    }
                }
                return -1;
            }

            public void StopMovement()
            {
                _isStopped = true;
                _selfBody.velocity = Vector2.zero;
            }
            public void ResumeMovement()
            {
                _isStopped = false;
                _selfBody.velocity = _selfVelocity;
            }

            private void OnCollisionStay2D(Collision2D collision)
            {
                if (collision.gameObject.CompareTag("TvWall_DV"))
                {
                    PushBackBoxcast(0.05f);
                }
            }
            private void OnCollisionEnter2D(Collision2D collision)//DESTROY SELF
            {
                if (_isStopped)
                {
                    return;
                }
                _selfBody.velocity = _selfVelocity;
                if (collision.gameObject.CompareTag("Corner_DV"))
                {
                    PlayerController.CanScoreLogo = false;
                    PlayerController.CanGrab = false;

                    tvController.ResetWinCorners(collision.gameObject);

                    PlayerController.IncreseScore(tvController.BounceCount);
                    tvController.ResetBounceCount();

                    PlayerController.DestroyAllDots();
                    StopAllCoroutines();
                    Destroy(_copyObject.gameObject);
                    Destroy(gameObject);
                    return;
                }

                if (PlayerController.IsGrabbing)
                {
                    if (collision.gameObject.CompareTag("SideDown_DV"))
                    {
                        StoreReflecetOnHitSide(collision);
                        PlayerController.ForceRelease(0);
                    }
                    else if (collision.gameObject.CompareTag("SideLeft_DV"))
                    {
                        StoreReflecetOnHitSide(collision);
                        PlayerController.ForceRelease(1);
                    }
                    else if (collision.gameObject.CompareTag("SideUp_DV"))
                    {
                        StoreReflecetOnHitSide(collision);
                        PlayerController.ForceRelease(2);
                    }
                    else if (collision.gameObject.CompareTag("SideRight_DV"))
                    {
                        StoreReflecetOnHitSide(collision);
                        PlayerController.ForceRelease(3);
                    }
                }

                if (!PlayerController.IsGrabbing && !tvController.IsReleaseFlashing)
                {
                    if (collision.gameObject.CompareTag("SideDown_DV"))
                    {
                        ReflecetOnHitSide(collision);
                        DetectionRaycast();
                        tvController.UpdateCornerColor();
                        tvController.BounceShift(0);
                    }
                    else if (collision.gameObject.CompareTag("SideLeft_DV"))
                    {
                        ReflecetOnHitSide(collision);
                        DetectionRaycast();
                        tvController.UpdateCornerColor();
                        tvController.BounceShift(1);
                    }
                    else if (collision.gameObject.CompareTag("SideUp_DV"))
                    {
                        ReflecetOnHitSide(collision);
                        DetectionRaycast();
                        tvController.UpdateCornerColor();
                        tvController.BounceShift(2);
                    }
                    else if (collision.gameObject.CompareTag("SideRight_DV"))
                    {
                        ReflecetOnHitSide(collision);
                        DetectionRaycast();
                        tvController.UpdateCornerColor();
                        tvController.BounceShift(3);
                    }
                    if (collision.gameObject.CompareTag("TvWall_DV"))
                    {
                        PlayerController.CanGrab = true;
                        tvController.IsReleaseFlashing = false;
                        ChangeRandomColor();
                        if (PlayerController.CanScoreLogo)
                        {
                            tvController.ResetLineCornerColor();
                            tvController.LightUpEyes();
                            tvController.IncreaseBounceCount();
                            tvController.ReleaseOppositeCorners();
                        }
                    }
                }
                else
                {
                    tvController.ResetBounceCount();
                }
            }
            void DebugDrawBoxCast(Vector2 origin, Vector2 size)
            {
                Vector2 topLeft = origin + new Vector2(-size.x / 2, size.y / 2);
                Vector2 topRight = origin + new Vector2(size.x / 2, size.y / 2);
                Vector2 bottomLeft = origin + new Vector2(-size.x / 2, -size.y / 2);
                Vector2 bottomRight = origin + new Vector2(size.x / 2, -size.y / 2);

                Debug.DrawLine(topLeft, topRight, Color.yellow,3f);
                Debug.DrawLine(topRight, bottomRight, Color.yellow, 3f);
                Debug.DrawLine(bottomRight, bottomLeft, Color.yellow, 3f);
                Debug.DrawLine(bottomLeft, topLeft, Color.yellow,3f);
            }
            private void PushBackBoxcast(float _deltaDist)
            {
                int failTick = 1; //push on first try
                bool _isClear = false;
                Vector3 _checkDeltaPos = new Vector3();
                Vector3 _centerDir = -(transform.position - tvController.GetTvCenter()).normalized;
                while (!_isClear)
                {
                    _centerDir = -(transform.position - tvController.GetTvCenter()).normalized;
                    Debug.DrawRay(transform.position, _centerDir, Color.red, 2.5f);
                    //Debug.DrawLine(transform.position, tvController.GetTvCenter(), Color.magenta, 2.5f);
                    _checkDeltaPos = (failTick * _deltaDist) * _centerDir;
                    _isClear = true;
                    failTick++;
                    if (failTick > 1000000)
                    {
                        Debug.LogError("While loop in PushBoxCast");
                        return;
                    }
                    RaycastHit2D[] _boxHit = Physics2D.BoxCastAll(transform.position+_checkDeltaPos, _selfCollider.size, 0,Vector2.zero);
                    if (DebugMode){DebugDrawBoxCast(transform.position + _checkDeltaPos, _selfCollider.size); }
                    foreach (RaycastHit2D hit in _boxHit)
                    {
                        if (hit.collider != null && hit.collider.tag == "TvWall_DV")
                        {
                            _isClear = false;
                        }
                    }
                }
                transform.position += _checkDeltaPos;
            }
            private void ChangeRandomColor()
            {
                // Generate a random hue value
                float randomHue = Random.Range(0f, 1f);

                // Set the new color with the same saturation and value, but a random hue
                Color newColor = Color.HSVToRGB(randomHue, 1f, 1f);

                // Apply the new color to the SpriteRenderer
                _sprRend.color = newColor;
                tvController.NewCornerColor(newColor);
            }
            private void ReflecetOnHitSide(Collision2D collision)
            {
                _selfVelocity = Vector2.Reflect(_selfVelocity, -collision.GetContact(0).normal);
                _selfBody.velocity = _selfVelocity;
            }
            private void StoreReflecetOnHitSide(Collision2D collision)
            {
                _selfVelocity = Vector2.Reflect(_selfVelocity, -collision.GetContact(0).normal);
            }

            public Vector3 debug1;
            public Vector3 debug2;
            public Vector3 debug3;
            public Vector3 debug4;
            void DetectionRaycast()
            {
                
                float minX = _selfCollider.bounds.min.x;
                float minY = _selfCollider.bounds.min.y;
                float maxX = _selfCollider.bounds.max.x;
                float maxY = _selfCollider.bounds.max.y;

                Vector2 bottomLeft = new Vector2(maxX, minY);
                Vector2 bottomRight = new Vector2(minX, minY);
                Vector2 upLeft = new Vector2(minX, maxY);
                Vector2 upRight = new Vector2(maxX, maxY);

                float _currShortestDist = float.MaxValue;
                _currHitSide = -1;

                FindBounceSide(bottomLeft, 0, _currShortestDist, _currHitSide, out _currHitSide, out _currShortestDist, out ShortestVertex);
                FindBounceSide(bottomRight, 1, _currShortestDist, _currHitSide, out _currHitSide, out _currShortestDist, out ShortestVertex);
                FindBounceSide(upLeft, 2,_currShortestDist, _currHitSide, out _currHitSide, out _currShortestDist, out ShortestVertex);
                FindBounceSide(upRight, 3, _currShortestDist, _currHitSide, out _currHitSide, out _currShortestDist, out ShortestVertex);

                //print(_currHitSide.ToString() +"  "+ _currShortestDist.ToString());
                
                tvController.ShowBounceSide(_currHitSide);
            }



            private int CheckSides(RaycastHit2D _rayhit)
            {
                switch (_rayhit.collider.tag)
                {
                    case ("SideDown_DV"):
                        {
                            return 0;
                        }
                    case ("SideLeft_DV"):
                        {
                            return 1;
                        }
                    case ("SideUp_DV"):
                        {
                            return 2;
                        }
                    case ("SideRight_DV"):
                        {
                            return 3;
                        }
                }
                return -1;
            }
            private void FindBounceSide(Vector2 _corner, int _cornerIndex, float _currShortestDist, int _currHitSide, out int _newHitSide, out float _newShortestDist, out int _shortestVertex)
            {
                RaycastHit2D[] _hitAll = Physics2D.RaycastAll(_corner, _selfVelocity);//_selfBody.velocity);
                foreach (RaycastHit2D rayhit in _hitAll)
                {
                    int _currSide = CheckSides(rayhit);
                    if (_currSide != -1)
                    {
                        float _dist = Vector2.Distance(_corner, rayhit.point);
                        if (DebugMode) { Debug.DrawLine(_corner, rayhit.point, Color.white, 3); }
                        if (_dist < _currShortestDist)
                        {
                            _newShortestDist = _dist;
                            _newHitSide = _currSide;
                            _shortestVertex = _cornerIndex;
                            if (DebugMode) { Debug.DrawLine(_corner, rayhit.point, Color.yellow, 3); }
                            return;
                        }
                    }
                }
                _newHitSide = _currHitSide;
                _newShortestDist = _currShortestDist;
                _shortestVertex = ShortestVertex;
            }
            private void OnDrawGizmos()
            {
                if (DebugMode)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(debug1, 0.1f);
                    Gizmos.DrawSphere(debug2, 0.1f);
                    Gizmos.DrawSphere(debug3, 0.1f);
                    Gizmos.DrawSphere(debug4, 0.1f);
                }
            }

            public void LaunchTrajectoryCopy()
            {
                _copyObject.gameObject.SetActive(true);
                _copyObject.ResetLife();
                _copyObject.Launch(_tvParent, transform, _dotParent, _selfBody.velocity, trajectoryLife,trajectorySpeedMulti);
            }
            public void ResetCopy()
            {
               _copyObject.SetNewLife(transform.position, _selfVelocity);
            }
            public void SetValues(Transform _tvParent, Transform _copyParent, Transform _dotParent, Transform _ghostParent,
                TvController_DV _tvController, GameObject _copyPrefab, int _life, float _slowRate, float _trajspdMulti)
            {
                slowRate = _slowRate;
                this._dotParent = _dotParent;
                this._tvParent = _tvParent;
                this._copyParent = _copyParent;
                this._copyPrefab = _copyPrefab;
                this._ghostParent = _ghostParent;

                trajectoryLife = _life;
                trajectorySpeedMulti = _trajspdMulti;

                tvController = _tvController;
            }
            private bool CheckVertexOnArea() //Unused
            {
                float minX = _selfCollider.bounds.min.x;
                float minY = _selfCollider.bounds.min.y;
                float maxX = _selfCollider.bounds.max.x;
                float maxY = _selfCollider.bounds.max.y;

                Vector2 bottomLeft = new Vector2(maxX, minY);
                Vector2 bottomRight = new Vector2(minX, minY);
                Vector2 upLeft = new Vector2(minX, maxY);
                Vector2 upRight = new Vector2(maxX, maxY);

                //comment to disable ref
                //if (CheckControlArea(bottomLeft)){return false;}
                //if (CheckControlArea(bottomRight)) { return false; }
                //if (CheckControlArea(upRight)) { return false; }
                //if (CheckControlArea(upLeft)) { return false; }
                return true;
            }
            private bool CheckControlArea(Vector2 _point) //Unused
            {
                Collider2D[] colliders = Physics2D.OverlapPointAll(_point);
                foreach (Collider2D _collider in colliders)
                {
                    if (_collider.isTrigger && _collider.tag == "ControlArea")
                    {
                        return true;
                    }
                }
                return false;
            }

        }
    }
}