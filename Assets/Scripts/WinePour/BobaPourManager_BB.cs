using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobaPourManager_BB : MonoBehaviour
{
    [Header ("Tuning&!!!")]
    public bool StopMovement;
    public bool HasWon;
    [SerializeField] private int winTick = 70;
    [SerializeField] private int currTick;
    public float checkPercentMin; //0.97
    public float checkPercentMax; //1.03

    [Header("Cups")]
    public float FillPercent;
    public BobaCup_BB[] CupArr;
    [SerializeField] private int cupIndex;
    private BobaCup_BB _selectedCup;
    public Transform CupPosition;
    [HideInInspector] public AnimationCurve PourRateCurve;

    [Header("Button")]
    public PourButton_BB PourButton;

    [Header ("Components")]
    public SpriteRenderer GreenVolume;
    private GreenVolumnBehavior_BB LiquidBehavior;
    private float _greenHeight;
    public float GreenTopPosition;
    public SpriteRenderer FoamVolume;
    private FoamVolumnBehavior_BB FoamBehavior;
    private float _foamHeight;
    public float FoamTopPosition;
    public SpriteRenderer FillRangeSpr;
    private float _glassTotalHeight;

    [Header("EggBubble")]
    public SpriteRenderer BubbleSpriteRend;
    private RedoBubble_BB _redoBubble;
    public Sprite BubbleIdle;
    public Sprite BubbleChecking;
    public Sprite BubbleAngry;
    public Sprite BubbleHappy;
    [Header("Eggthulhu")]
    public SpriteRenderer EggthulhuSpriteRend;
    public Sprite EggthulhuIdle;
    public Sprite EggthulhuChecking;
    public Sprite EggthulhuAngry;
    public Sprite EggthulhuHappy;

    // Start is called before the first frame update
    void Awake()
    {
        LiquidBehavior = GreenVolume.GetComponent<GreenVolumnBehavior_BB>();
        FoamBehavior = FoamVolume.GetComponent<FoamVolumnBehavior_BB>();
        _redoBubble = BubbleSpriteRend.gameObject.GetComponent<RedoBubble_BB>();
    }

    public void StartNew() //OVERRIDE
    {
        StopMovement = false;
        HasWon = false;
        BubbleSpriteRend.color = new Color(1, 1, 1, 1);
        _redoBubble.enabled = false;
        LiquidBehavior.ResetScale();
        FoamBehavior.ResetFoam();
        SelectBobaCup();
        EggthulhuSpriteRend.sprite = EggthulhuIdle;
        BubbleSpriteRend.sprite = BubbleIdle;
    }

    public void StartClose()
    { }

    public void Close() //OVERRIDE
    {
        StopMovement = false;
        HasWon = false;
        _redoBubble.enabled = false;
        Destroy(_selectedCup.gameObject);
        LiquidBehavior.ResetScale();
        FoamBehavior.ResetFoam();
        PourButton.ButtonReset();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (HasWon)
        {
            return;
        }
        GreenTopPosition = GreenVolume.bounds.max.y;
        FoamTopPosition = FoamVolume.bounds.max.y;
        _greenHeight = GreenVolume.bounds.max.y - GreenVolume.bounds.min.y;
        _foamHeight = FoamVolume.bounds.max.y - FoamVolume.bounds.min.y;
        _glassTotalHeight = FillRangeSpr.bounds.max.y - FillRangeSpr.bounds.min.y;
        FillPercent = (_greenHeight + _foamHeight) / (_glassTotalHeight);

        _selectedCup.UpdateBobaAmplitude(FillPercent*0.1f); //Use only 10% of the percent(0 to 1)
        CheckWin();
    }
    public float GetCurrentPourRate()
    {
        return PourRateCurve.Evaluate(FillPercent);
    }
    public void SelectBobaCup()
    {
        cupIndex = Random.Range(0, CupArr.Length);
        _selectedCup = Instantiate(CupArr[cupIndex], CupPosition.position, transform.rotation, this.transform);
        if (_selectedCup.HaveSleeve)
        {
            if (Random.Range(0f, 1f) >= 0.95f)
            {
                _selectedCup.SleeveObj.SetActive(false);
            }
        }
        FillRangeSpr = _selectedCup.TargetRangeSprite;
        _selectedCup.TargetRangeSprite.enabled = false;
        PourRateCurve = _selectedCup.CupCurve;
        SetContentPosition();
    }
    public void SetContentPosition()
    {
        GreenVolume.transform.position = new Vector3(GreenVolume.transform.position.x, FillRangeSpr.bounds.min.y, 0);
    }

    public void RedoPour()
    {
        BubbleSpriteRend.color = new Color(1, 1, 1, 1);
        _redoBubble.enabled = false;
        BubbleSpriteRend.sprite = BubbleIdle;
        EggthulhuSpriteRend.sprite = EggthulhuIdle;

        Destroy(_selectedCup.gameObject);
        SelectBobaCup();
        FoamBehavior.ResetFoam();
        LiquidBehavior.ResetScale();
        StopMovement = false;

        PourButton.ButtonReset();
    }

    private void CheckWin()
    {
        if (FillPercent > checkPercentMin && FillPercent < checkPercentMax)
        {
            EggthulhuSpriteRend.sprite = EggthulhuChecking;
            BubbleSpriteRend.sprite = BubbleChecking;
            currTick--;
            if (currTick < 0)
            {
                EggthulhuSpriteRend.sprite = EggthulhuHappy;
                BubbleSpriteRend.sprite = BubbleHappy;
                HasWon = true; //WIN
                StopMovement = true;
            }
        }
        else
        {
            currTick = winTick;
        }
        if (FillPercent > checkPercentMax)
        {
            StopMovement = true;
            EggthulhuSpriteRend.sprite = EggthulhuAngry;
            BubbleSpriteRend.sprite = BubbleAngry;
            _redoBubble.enabled = true;
            if (!PourButton.OverflowParticle.gameObject.activeSelf) //maybe
            {
                PourButton.StartOverflowParticle();
            }
        }
    }
}
