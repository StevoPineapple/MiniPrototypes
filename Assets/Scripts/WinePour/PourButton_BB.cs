using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PourButton_BB : MonoBehaviour
{
    [Header("drops")]
    public BobaDrop_BB BobaDrop;
    public GameObject DropHolder;
    private List<GameObject> _dropList = new List<GameObject>();
    public int MinDropAmount; //2
    public int MaxDropAmount; //4
    public float MaxDropSize;
    public float MinDropSize;
    public float PosHAdjust; //horizontal random on spawn
    public float PosVAdjust; //vertical random on spawn

    [Header("pour")]
    public SpriteRenderer StreamSpriteRend;
    private Vector3 _streamSpriteInitPos;
    public bool IsDetached;
    private float _detachAcc = 0;

    public GreenVolumnBehavior_BB GreenVolumn;
    public FoamVolumnBehavior_BB FoamVolumn;
    public BobaPourManager_BB PourManager;

    private float _pourYScale;
    private float _pourAcc;
    public float PourYMaxScale;
    public float PourXMacScale;
    public float PourGravity;

    public ParticleSystem OverflowParticle;

    [Header("Button")]
    public SpriteRenderer ButtonSprite;
    public Sprite ButtonHover;
    public Sprite ButtonIdle;
    public Sprite ButtonPush;

    [Header("Machine")]
    [SerializeField] private bool _turnOn;
    [SerializeField] private bool _turningOn;
    [SerializeField] private bool _ButtonPush;

    public SpriteRenderer MachineSprite;
    private Vector3 _machineSpriteInitPos;
    public Transform MachinePourPoint;
    private float machinePourRate;
    public float MachinePourRateInit;
    public float MachinePourRateMultiplier;
    public int MachineTurnOnTickMax;
    public int MachineTurnOnTick;


    private void Start()
    {
        OverflowParticle.gameObject.SetActive(false);
        _streamSpriteInitPos = MachinePourPoint.position;
        _machineSpriteInitPos = MachineSprite.gameObject.transform.position;

        machinePourRate = MachinePourRateInit;
        PourYMaxScale = 5f;
        PourGravity = 0.02f;

        MachineTurnOnTick = MachineTurnOnTickMax;
    }
    void FixedUpdate()
    {
        if (_turningOn)
        {
            ShakeMachine(0);
            if (MachineTurnOnTick > 0)
                MachineTurnOnTick--;
        }

        if (_turningOn && MachineTurnOnTick <= 0) // turn on check
        {
            _turningOn = false;
            MachineSprite.gameObject.transform.position = _machineSpriteInitPos;
            if (_ButtonPush)
            {
                IsDetached = false;//
                _detachAcc = 0; //
                StreamSpriteRend.transform.position = _streamSpriteInitPos; //
                StreamSpriteRend.transform.localScale = new Vector3(0, 0, 1);
                _turnOn = true;
            }
            else
            {
                _turnOn = false;
                int dropsCount = Random.Range(MinDropAmount, MaxDropAmount + 1);
                while (dropsCount > 0)
                {
                    CreateDrop();
                    dropsCount--;
                }
                MachineTurnOnTick = MachineTurnOnTickMax;
            }
        }

        if (_turnOn)
        {
            ShakeMachine(1);
            PourLiquid(machinePourRate);
            machinePourRate *= MachinePourRateMultiplier;
            if (!_ButtonPush)
            {
                MachineTurnOnTick = MachineTurnOnTickMax;
                machinePourRate = MachinePourRateInit;
                _pourAcc = 0;
                _pourYScale = 0;

                _turnOn = false;
                //StreamSpriteRend.transform.localScale = new Vector3(MachinePourRate, _pourYScale, 1);
            }
        }
        if (IsDetached)
        {
            _detachAcc -= PourGravity;
            StreamSpriteRend.transform.position += new Vector3(0, _detachAcc, 0);
        }
        PourOnContact();
    }

    private void OnMouseDown()
    {
        _turningOn = true;
    }
    private void OnMouseEnter()
    {
        ButtonSprite.sprite = ButtonHover;
    }
    private void OnMouseExit()
    {
        ButtonSprite.sprite = ButtonIdle;
    }
    private void OnMouseDrag()
    {
        ButtonSprite.sprite = ButtonPush;
        _ButtonPush = true;
    }
    private void OnMouseUp()
    {
        ButtonSprite.sprite = ButtonIdle;
        _ButtonPush = false;
        IsDetached = true;//
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _ButtonPush = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            IsDetached = false;//
            _detachAcc = 0; //
            StreamSpriteRend.transform.position = _streamSpriteInitPos; //
            StreamSpriteRend.transform.localScale = new Vector3(0, 0, 1);

            _turningOn = true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            _ButtonPush = false;
            IsDetached = true;//
        }
    }
    void ShakeMachine(int level)
    {
        if (level == 0)
        {
            MachineSprite.transform.position = new Vector3(_machineSpriteInitPos.x + Random.Range(-0.1f, 0.1f),
                _machineSpriteInitPos.y + Random.Range(-0.1f, 0.1f), _machineSpriteInitPos.z);
        }
        else if (level == 1)
        {
            MachineSprite.transform.position = new Vector3(_machineSpriteInitPos.x + Random.Range(-0.02f, 0.02f),
                _machineSpriteInitPos.y + Random.Range(-0.02f, 0.02f), _machineSpriteInitPos.z);
        }
    }
    void CreateDrop()
    {
        float size = Random.Range(MinDropSize, MaxDropSize);
        BobaDrop_BB _bobaDrop = Instantiate(BobaDrop, MachinePourPoint.position, transform.rotation, DropHolder.transform);
        _bobaDrop.SetInitialTransform(size, new Vector3(Random.Range(-PosHAdjust, PosHAdjust), Random.Range(0, -PosVAdjust), 0));
        _bobaDrop.PourButton = this;
        _bobaDrop.PourRate = PourManager;
        _dropList.Add(_bobaDrop.gameObject);
    }
    IEnumerator DoTurnOn()
    {
        _turningOn = true;
        yield return new WaitForSeconds(0.3f);
        _turningOn = false;
        _turnOn = true;
    }

    public void PourIncomplete(float rate)
    {
        if (PourManager.StopMovement)
        {
            var _emission = OverflowParticle.emission;
            float _rate = Mathf.Clamp(_emission.rateOverTimeMultiplier + rate * 0.02f * PourManager.GetCurrentPourRate(), 5, 30);
            _emission.rateOverTime = _rate;
            return;
        }
        GreenVolumn.GrowVolume(rate * 0.04f * PourManager.GetCurrentPourRate()); //10
        FoamVolumn.GrowFoam(rate * 0.01f * PourManager.GetCurrentPourRate()); //2.5
        FoamVolumn.AddFreshness(rate * 0.001f * PourManager.GetCurrentPourRate());//This is off-balanced, so droplet will give more freshness
        //and will give less foam
        print("drop");
    }

    void PourLiquid(float pourRatio)
    {
        if (_pourYScale < PourYMaxScale)
        {
            _pourAcc += PourGravity;
            _pourYScale += _pourAcc;
        }

        //print(wineSpr.size.x);

        //Vector3 pourWidthPos = Vector3.Lerp(bottomRef.position, topRef.position, pourRatio);
        //while (wineSpr.bounds.min.x > pourSizeX)
        Vector3 scale = StreamSpriteRend.transform.localScale;

        StreamSpriteRend.transform.localScale = new Vector3(Mathf.Clamp(machinePourRate, 0, PourXMacScale), _pourYScale, scale.z);
        //PourOnContact();
        //wineSpr.transform.rotation = Quaternion.Euler(0, 0, 0); // lock rotation

        //wineTopSpr.transform.rotation = Quaternion.Euler(0, 0, 0); // lock rotation
        //wineTopSpr.transform.localScale = new Vector3(wineSpr.transform.localScale.x, wineTopSprInitScale, wineSpr.transform.localScale.z);
    }
    void PourOnContact()
    {
        if (StreamSpriteRend.bounds.min.y <= PourManager.FoamTopPosition && StreamSpriteRend.bounds.max.y >= PourManager.FoamTopPosition)
        {
            if (PourManager.StopMovement)
            {
                var _emission = OverflowParticle.emission;
                float _rate = Mathf.Clamp(_emission.rateOverTimeMultiplier + 0.05f * machinePourRate * PourManager.GetCurrentPourRate(), 5, 30);
                _emission.rateOverTime = _rate;
                return;
            }
            GreenVolumn.GrowVolume(0.1f * machinePourRate * PourManager.GetCurrentPourRate()); //5
            FoamVolumn.GrowFoam(0.01f * machinePourRate * PourManager.GetCurrentPourRate()); //1
            FoamVolumn.AddFreshness(0.1f * machinePourRate * PourManager.GetCurrentPourRate());//
        }
    }

    public void StartOverflowParticle()
    {
        OverflowParticle.gameObject.SetActive(true);
        var _emission = OverflowParticle.emission;
        _emission.rateOverTimeMultiplier = 5;
    }
    private void StopOverflowParticle()
    {
        var _emission = OverflowParticle.emission;
        _emission.rateOverTimeMultiplier = 5;
        OverflowParticle.gameObject.SetActive(false);
    }
    public void ButtonReset()
    {
        _ButtonPush = false;
        _turningOn = false;
        _turnOn = false;
        IsDetached = false;

        //Reset Pour Sprite
        IsDetached = false;//
        _detachAcc = 0; //
        StreamSpriteRend.transform.position = _streamSpriteInitPos; //
        StreamSpriteRend.transform.localScale = new Vector3(0, 0, 1);

        StopOverflowParticle();
        DestryDroplets();
        ResetPourRate();
    }
    private void DestryDroplets()
    {
        foreach (GameObject _drop in _dropList)
        {
            Destroy(_drop);
        }
        _dropList.Clear();
    }
    private void ResetPourRate()
    {
        machinePourRate = MachinePourRateInit;
    }
}
