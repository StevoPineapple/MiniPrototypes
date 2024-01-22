using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RingTossTaskBehavior : MonoBehaviour //Task Behavior
{
    public bool HasWon;
    [Header ("Prize")]
    public GameObject PrizeObject;
    public GameObject PrizeHolder;

    public float PrizeNumber;
    [SerializeField] private int prizeNumberMax;

    [SerializeField] private int normalPrizeNumber;
    [SerializeField] private int redPrizeNumber;
    [SerializeField] private int spPrizeNumber;

    [SerializeField] private int normalScore;
    [SerializeField] private int redScore;
    [SerializeField] private int spScore;

    public int patternIndex;
    
    public PrizeBehavior_RT[] PrizeArr;
    public Sprite[] SpecialSpriteArr;

    public int ScoreTarget;
    public int CurrentScore;

    [Header ("SpawnRadius")]
    public Transform SpawnCircleCenter;
    public float SpawnCircleRadius; 

    [Header ("Component")]
    public UIManager_RT RTUIManager;
    public PlayerController_RT PlayerController;
    public RingObject_RT Ring;

    [Header ("Debug")]
    public bool GizmoDebug;

    [Header("MiniManager")]
    public RingTossMiniManager RTMM;

    public void StartOpen() //OVERRIDE
    {
        HasWon = false;
        RandomizePrizes(ChoosePattern());
    }
    public void StartClose() //OVERRIDE
    {
        CurrentScore = 0;
        ScoreTarget = 0;

        PlayerController.CloseGame();
        Ring.CloseGame();
        foreach (PrizeBehavior_RT _prize in PrizeArr)
        {
            if (_prize != null)
            {
                Destroy(_prize.gameObject);
            }
        }
    }
    private void Update()
    {
        //If point > target
        //Win
    }
    public void AddPoint(int point)
    {
        //Call in player
        //if CurrentScore>Target
        //(Win!!!)
    }
    public void AddPoint(string type)
    {
        //Call in player
        switch (type)
        {
            case ("nor"):
                {
                    CurrentScore += normalScore;
                    break;
                }
            case ("red"):
                {
                    CurrentScore += redScore;
                    break;
                }
            case ("sp"):
                {
                    CurrentScore += spScore;
                    break;
                }
        }
    }
    private int ChoosePattern()
    {
        //Pattern Index 0: Normal, balanced random
        //Pattern Index 1: Minus point group
        //Pattern Index 2: Many small points
        //Pattern Index 3: Inversed Normal
        if (Random.Range(0f, 1f) > 0.8f)
        {
            patternIndex = 0;
        }
        else
        {
            patternIndex = Random.Range(1, 4);
        }
        return patternIndex;
    }
    private void RandomizePrizes(int _pattern)
    {
        int prizeNumberTens = 0;
        int _pointsForTens = 0;
        switch (_pattern)
        {
            case (0):
                {
                    prizeNumberTens = Random.Range(7, 9);


                    normalScore = 1;
                    redScore = -2;
                    spScore = 5;

                    //7:2:1 = 8pts for 10
                    normalPrizeNumber = 7 * prizeNumberTens;
                    redPrizeNumber = 2 * prizeNumberTens;
                    spPrizeNumber = 1 * prizeNumberTens;
                    //This is how much points is needed for the player per 10 prizes
                    //Should be lower than actual point produced by 10 prizes.
                    _pointsForTens = 6;
                    break;
                }
            case (1):
                {
                    prizeNumberTens = Random.Range(7, 9);
                    normalScore = 1;
                    redScore = -2;
                    spScore = 10;

                    //1:8:1 = 1-16+16 1pt for 10
                    normalPrizeNumber = 1 * prizeNumberTens;
                    redPrizeNumber = 7 * prizeNumberTens;
                    spPrizeNumber = 2 * prizeNumberTens;
                    
                    _pointsForTens = 6;
                    break;
                }
            case (2):
                {
                    prizeNumberTens = Random.Range(9, 11);
                    normalScore = 1;
                    redScore = -1;
                    spScore = 0;

                    //9:1:0 = 9-1 8pts for 10
                    normalPrizeNumber = 9 * prizeNumberTens;
                    redPrizeNumber = 1 * prizeNumberTens;
                    spPrizeNumber = 0 * prizeNumberTens;
                    
                    _pointsForTens = 6;
                    break;
                }
            case (3): //Inverse Normal
                {
                    prizeNumberTens = Random.Range(7, 9);

                    normalScore = -1;
                    redScore = 8;
                    spScore = -1;

                    //7:2:1 = -7+16+-1 = 8
                    normalPrizeNumber = 7 * prizeNumberTens;
                    redPrizeNumber = 2 * prizeNumberTens;
                    spPrizeNumber = 1 * prizeNumberTens;

                    _pointsForTens = 5;
                    break;
                }
        }
        ScoreTarget = prizeNumberTens * _pointsForTens; // Set Max Score

        RTUIManager.SetScoreTarget(ScoreTarget);
        RTUIManager.AddNormalScoreRule(normalScore);
        RTUIManager.AddRedScoreRule(redScore);
        RTUIManager.AddScoreRuleSpecial(SpecialSpriteArr[Random.Range(0, SpecialSpriteArr.Length)], spScore);

        prizeNumberMax = prizeNumberTens * 10;
        PrizeArr = new PrizeBehavior_RT[prizeNumberMax];
        for (int i = 0; i < prizeNumberMax; i++) //Spawn a Bunch
        {
            float _angle = Random.Range(0, 2 * Mathf.PI);
            Vector2 circlePosition = new Vector2(
            SpawnCircleCenter.position.x + Random.Range(0f,SpawnCircleRadius) * Mathf.Cos(_angle),
            SpawnCircleCenter.position.y + Random.Range(0f,SpawnCircleRadius) * Mathf.Sin(_angle)
            );
            GameObject _prizeObject = Instantiate(PrizeObject, circlePosition, transform.rotation, PrizeHolder.transform);
            PrizeArr[i] = _prizeObject.GetComponent<PrizeBehavior_RT>();
        }

        PrizeArr = PrizeArr.OrderBy(_prize => _prize.gameObject.transform.position.y).ToArray();
        System.Array.Reverse(PrizeArr);

        for (int i = 0; i < redPrizeNumber; i++) //Change to Red
        {
            int changeToSpIndex = Random.Range(0, prizeNumberMax);
            PrizeBehavior_RT prizeToChange = PrizeArr[changeToSpIndex].GetComponent<PrizeBehavior_RT>();
            prizeToChange.IsModified = true;
            prizeToChange.ChangeToRed();
        }

        for (int i = 0; i < spPrizeNumber; i++) //Change to Special
        {
            int changeToSpIndex = Random.Range(0, prizeNumberMax);
            PrizeBehavior_RT prizeToChange = PrizeArr[changeToSpIndex].GetComponent<PrizeBehavior_RT>();
            while (prizeToChange.IsModified)
            {
                changeToSpIndex = Random.Range(0, prizeNumberMax);
                prizeToChange = PrizeArr[changeToSpIndex].GetComponent<PrizeBehavior_RT>();
            }
            prizeToChange.IsModified = true;
            prizeToChange.ChangeToSpecial();
        }

        for (int i = 0; i < prizeNumberMax; i++)
        {
            PrizeArr[i].SetSortingOrder(i);
        }

    }

    public void WinCheck()
    {
        if (CurrentScore >= ScoreTarget)
        {
            RTMM.SetWin();
            HasWon = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (!GizmoDebug)
        {
            return;
        }
        for (int i = 0; i < prizeNumberMax; i++) //Spawn a Bunch
        {
            float _angle = Random.Range(0, 2 * Mathf.PI);
            Vector2 circlePosition = new Vector2(
            SpawnCircleCenter.position.x + Random.Range(0f, SpawnCircleRadius) * Mathf.Cos(_angle),
            SpawnCircleCenter.position.y + Random.Range(0f, SpawnCircleRadius) * Mathf.Sin(_angle)
            );
            Gizmos.DrawSphere(circlePosition, 1);
        }
    }
}
