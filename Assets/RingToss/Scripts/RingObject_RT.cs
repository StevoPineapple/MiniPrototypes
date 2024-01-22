using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingObject_RT : MonoBehaviour
{
    private CircleCollider2D _selfCollider;
    public SpriteRenderer SprRend;
    public bool IsTossed;
    public bool HasLanded;
    public bool CanGetPrize = true;
    public bool OnWater; //Landed on water but not check collision
    public Transform StartingPosition;

    [Header("Scale")]
    public float Gravity;
    public float FallSpeed;
    public float FallSpeedMax;
    public int AirTime;

    [Header("YMovement")]
    [SerializeField] private float yInitSpeed; //0.3f
    [SerializeField] private float yStrengthAdj; //0.3f
    [SerializeField] private float yStrengthMax; //0.3f
    [SerializeField] private float yAcc;
    [SerializeField] private float ySpeed;
    [SerializeField] private float ySpeedMin;

    [Header("XMovement")]
    [SerializeField] private float xAcc;
    [SerializeField] private float xStrengthMax; //0.75f
    [SerializeField] private float xStrengthAdj; //0.5f
    [SerializeField] private float xSpeed;

    [Header("Shadow")]
    public GameObject ShadowObject;
    [SerializeField] private float shadowInitGap;
    [SerializeField] private float shadowEndGap;
    private float _shadowTotalGap;
    [SerializeField] private float shadowGap;

    [Header("OnLanding")]
    [SerializeField] private AnimationCurve landingFloatCurve;
    [SerializeField] private float landingCurveTime;

    public CircleCollider2D WaterCollider;
    public CircleCollider2D WaterEdgeCollider;
    private float _slideRange;

    [SerializeField] private bool onWaterCenter;
    [SerializeField] private bool onWaterEdge;
    [SerializeField] private bool onPoolEdge;
    [SerializeField] private bool onNotFar;

    private float _moveToCenterSpeed;
    [SerializeField] private float moveLerp;
    [SerializeField] private AnimationCurve distanceAdjCurve; //for sliding
    private Vector3 _moveDirection;

    public float InitSize;
    public float HitWaterSize;

    public RingTossTaskBehavior RTManager;
    public List<GameObject> GotPrizeList = new List<GameObject>();
    void Start()
    {
        _selfCollider = GetComponent<CircleCollider2D>();
        _slideRange = WaterEdgeCollider.radius - WaterCollider.radius;
        ResetRing();
    }

    void FixedUpdate()
    {
        if (onWaterCenter)
        {
            FloatAfterLanding();
        }
        else if (onWaterEdge)
        {
            transform.Translate(_moveDirection * _moveToCenterSpeed);
            _moveToCenterSpeed = Mathf.Clamp(Mathf.Lerp(_moveToCenterSpeed, -0.01f, moveLerp - (0.3f * distanceAdjCurve.Evaluate(_slidePercent))), 0, 0.1f);
        }
        else if (onPoolEdge)
        {
            SprRend.gameObject.transform.Rotate(18, 18, 0);
            ShadowObject.transform.rotation = SprRend.gameObject.transform.rotation;
            transform.Translate(_moveDirection * _moveToCenterSpeed);
        }

        if (IsTossed)
        {
            //AirTime--;
            if (transform.localScale.x <= HitWaterSize)
            {
                if (!OnWater)
                {
                    Landing();
                }
                OnWater = true;
            }
            else
            {
                HandleYMovement();
                HandleXMovement();
                HandleShadow();
                FallSpeed = Mathf.Clamp(FallSpeed + Gravity, 0, FallSpeedMax);
                float _currscale = transform.localScale.x - FallSpeed;
                transform.localScale = new Vector3(_currscale, _currscale, _currscale);
            }
        }
        else
        {
            transform.localScale = new Vector3(InitSize, InitSize, InitSize);
        }
    }
    private Coroutine _landRoutine;
    private Coroutine _digestRoutine;
    private void HandleYMovement()
    {
        ySpeed = Mathf.Clamp(ySpeed + yAcc, ySpeedMin, 10); //No maximum
        transform.position += new Vector3(0, ySpeed, 0);
    }
    private void HandleXMovement()
    {
        xSpeed = Mathf.Lerp(xSpeed, 0, 0.1f);
        transform.position += new Vector3(xSpeed, 0,0);
    }
    private void HandleShadow()
    {
        ShadowObject.transform.position = transform.position + new Vector3(0, shadowGap, 0);
        ShadowObject.transform.localScale = transform.localScale;
        shadowGap = shadowEndGap+shadowInitGap * (1-(InitSize-transform.localScale.x) / (InitSize - HitWaterSize));
    }
    private Coroutine _collisionRoutine;
    private void Landing()
    {
        ShadowObject.transform.position = transform.position + new Vector3(0, shadowEndGap, 0);

        //particles and stuff
        _collisionRoutine = StartCoroutine(DoCheckCollisionOnFrame()); /////
    }
    private float _slidePercent;
    IEnumerator DoCheckCollisionOnFrame()
    {
        _selfCollider.enabled = true;
        yield return new WaitForFixedUpdate(); //wait for collider to check landingPos

        HasLanded = true;
        if (!onNotFar)
        {

            //////////////////////PLAY OUT AUDIO
            
            CanGetPrize = false;
            SprRend.color = new Color(0.5f, 0.5f, 0.5f, 0);
            _selfCollider.enabled = false;
            yield return new WaitForSeconds(1.1f);
            ResetRing();
            yield break;
        }

        if (onWaterCenter)
        {

            //////////////////////Splash

            CanGetPrize = true;
            yield return new WaitForFixedUpdate(); //wait for collider to check prize
        }
        else if (onWaterEdge) //Slide
        {

            //////////////////////Slide

            float _dis = Vector2.Distance(transform.localPosition, WaterCollider.transform.localPosition);
            float _rad = _selfCollider.radius / transform.localScale.x;
            _slidePercent = (1 - ((WaterEdgeCollider.radius - (_dis - _rad)) / _slideRange));
            _moveToCenterSpeed = distanceAdjCurve.Evaluate(_slidePercent);

            print((1 - ((WaterEdgeCollider.radius - (_dis - _rad)) / _slideRange)));
            _moveDirection = (WaterCollider.transform.position - transform.position).normalized;
            CanGetPrize = true;
            while (_moveToCenterSpeed > 0.02f)
            {
                yield return new WaitForFixedUpdate(); //check Prize
            }
        }
        else if (onPoolEdge)
        {

            //////////////////////BounceOff

            CanGetPrize = false;
            SprRend.color = new Color(0.5f, 0.5f, 0.5f, 1);
            _moveToCenterSpeed = 0.5f;
            _moveDirection = (transform.position- WaterCollider.transform.position).normalized;
        }
        else
        {
            CanGetPrize = false;
            SprRend.color = new Color(0.5f, 0.5f, 0.5f, 0);
        }
        _selfCollider.enabled = false;
        yield return new WaitForSeconds(1.1f);
        _digestRoutine = StartCoroutine(DoDigestPrize());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!HasLanded)
        {
            if (collision.gameObject.CompareTag("NotFar_RT"))
            {
                onNotFar = true;
            }
            if (collision.gameObject.CompareTag("Water_RT"))
            {
                onWaterCenter = true;
            }
            if (collision.gameObject.CompareTag("WaterEdge_RT"))
            {
                onWaterEdge = true;
            }
            if (collision.gameObject.CompareTag("PoolEdge_RT"))
            {
                onPoolEdge = true;
            }
        }
        if (!CanGetPrize)
        {
            return;
        }
        if (collision.gameObject.CompareTag("PrizeNormal_RT")||
            collision.gameObject.CompareTag("PrizeRed_RT")||
            collision.gameObject.CompareTag("PrizeSpecial_RT"))
        {
            collision.gameObject.GetComponent<PrizeBehavior_RT>().GotChosen((transform.position - collision.gameObject.transform.position).normalized,
                Vector2.Distance(transform.position, collision.gameObject.transform.position),transform,_selfCollider.radius);
            GotPrizeList.Add(collision.gameObject);
        }
    }
    IEnumerator DoDigestPrize()
    {
        foreach(GameObject _prizeObject in GotPrizeList)
        {
            if (_prizeObject.CompareTag("PrizeNormal_RT"))
            {
                RTManager.AddPoint("nor");
            }
            else if (_prizeObject.CompareTag("PrizeRed_RT"))
            {
                RTManager.AddPoint("red");
            }
            else if (_prizeObject.CompareTag("PrizeSpecial_RT"))
            {
                RTManager.AddPoint("sp");
            }
            Destroy(_prizeObject);
            yield return new WaitForFixedUpdate();
        }
        GotPrizeList.Clear();
        yield return new WaitForSeconds(0.1f); //pause
        RTManager.WinCheck();
        ResetRing();
    }
    private void ResetRing()
    {
        transform.localScale = new Vector3(InitSize, InitSize, InitSize);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.position = StartingPosition.position;

        ySpeed = yInitSpeed;
        FallSpeed = 0;

        _selfCollider.enabled = false;
        CanGetPrize = true;
        IsTossed = false;
        HasLanded = false;
        OnWater = false;

        ShadowObject.transform.position = transform.position + new Vector3(0, shadowInitGap, 0);
        _shadowTotalGap = shadowInitGap - shadowEndGap;

        _moveDirection = Vector3.zero;
        _moveToCenterSpeed = 0;

        onWaterCenter = false;
        onPoolEdge = false;
        onWaterEdge = false;
        onNotFar = false;

        SprRend.color = new Color(1f, 1f, 1f, 1);
        SprRend.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        SprRend.gameObject.transform.localPosition = Vector3.zero;
        SprRend.gameObject.transform.localScale = Vector3.one;
        ShadowObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        landingCurveTime = 0;        
    }
    public void Toss(float SideStrength, float FrontStrength)
    {
        Mathf.Clamp(SideStrength, 0, xStrengthMax);
        Mathf.Clamp(FrontStrength, 0, yStrengthMax);
        ySpeed = yInitSpeed + FrontStrength * yStrengthAdj;
        xSpeed = SideStrength*xStrengthAdj;
        IsTossed = true;
    }
    public void CloseGame()
    {
        if (_digestRoutine != null)
        {
            StopCoroutine(_digestRoutine);
        }
        if (_collisionRoutine != null)
        {
            StopCoroutine(_collisionRoutine);
        }
    }
    private void FloatAfterLanding()
    {
        SprRend.gameObject.transform.localPosition = new Vector3(0, landingFloatCurve.Evaluate(landingCurveTime),0);
        float _scale = 1+landingFloatCurve.Evaluate(landingCurveTime);
        SprRend.gameObject.transform.localScale = new Vector3(_scale,_scale,_scale);
        landingCurveTime += Time.fixedDeltaTime;
    }
}
