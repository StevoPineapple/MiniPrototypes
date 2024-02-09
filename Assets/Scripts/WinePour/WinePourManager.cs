using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinePourManager : MiniGameManager
{
    public BobaPourManager_BB Boba;

    public SpriteRenderer targetRangeRef;
    public GameObject targetMarker;
    public static GameObject TaargetRange;
    public static bool HasWon;
    public static bool HasLost;
    public GreenVolumnBehavior_BB wineVolume;
    public GlassFly glassObj;
    public SpriteRenderer drinker;
    public Sprite drinkerCalm;
    public Sprite drinkerAngry;

    public GameObject LoseObject;

    float targetPos;
    float targetMaxY;
    float targetMinY;
    float targetRangeSize; //Difficulty: 1 = easist
    // Start is called before the first frame update
    public override void StartGame()
    {
        Init();
        Boba.StartNew();
        /*if (!firstTime)
        {
            wineVolume.ResetScale();
            //foam reset
            glassObj.GetComponent<GlassFly>().ResetTrans();
        }*/
        //LoseObject.SetActive(false);
        //HasWon = false;
        //HasLost = false;

        //wineVolume.wineVolumeTop = 0;
        //foam reset to 0;

        //drinker.sprite = drinkerCalm;

        //if(TargetRange==null)
        //    TargetRange = transform.Find("TargetFamily/TargetRange").gameObject;

        //targetRangeRef.enabled = true;
        //targetMaxY = targetRangeRef.bounds.max.y;
        //targetMinY = targetRangeRef.bounds.min.y;
        //targetRangeRef.enabled = false;

        //firstTime = false;
        //SetTarget();
    }

    // Update is called once per frame
    public override void ResetGame()
    {
        HasWon = false;
        Boba.Close();
        //HasLost = false;
        //HasWon = false;
        //yield return new WaitForSeconds(GameSelect.GameCloseTime-0.1f);

        //StartCoroutine(DoResetGame());
    }

    IEnumerator DoResetGame()
    {
        yield return null;
    }

    private void Update()
    {
        if (HasWon)
        {
            return;
        }
        if (Boba.HasWon)
        {
            HasWon = true;
            StartCoroutine(DoSetWin());
        }
    }
    public override void SetWin()
    {
        StartCoroutine(DoSetWin());
    }
    public void SetLose()
    {
        HasLost = true;
        LoseObject.SetActive(true);
        GameSelectManager.QuitGame();
    }
    IEnumerator DoSetWin()
    {
        HasWon = true;
        WinObject.SetActive(true);
        yield return new WaitForSeconds(GameSelect.GameCloseTime);
        GameSelectManager.QuitGame();
    }
}