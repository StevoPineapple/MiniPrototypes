using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinePourManager : MiniGameManager
{
    public SpriteRenderer targetRangeRef;
    public GameObject targetMarker;
    public static GameObject TargetRange;
    public static bool HasWon;
    public static bool HasLost;
    public WineVolumnBehavior wineVolume;
    public GlassFly glassObj;
    public SpriteRenderer drinker;
    public Sprite drinkerCalm;
    public Sprite drinkerAngry;

    public GameObject LoseObject;

    bool firstTime = true;
    float targetPos;
    float targetMaxY;
    float targetMinY;
    float targetRangeSize; //Difficulty: 1 = easist
    // Start is called before the first frame update
    public override void StartGame()
    {
        Init();
        if (!firstTime)
        {
            wineVolume.ResetScale();
            glassObj.GetComponent<GlassFly>().ResetTrans();
        }
        LoseObject.SetActive(false);
        HasWon = false;
        HasLost = false;

        wineVolume.wineVolumeTop = 0;

    drinker.sprite = drinkerCalm;
        if(TargetRange==null)
            TargetRange = transform.Find("TargetFamily/TargetRange").gameObject;

        targetRangeRef.enabled = true;
        targetMaxY = targetRangeRef.bounds.max.y;
        targetMinY = targetRangeRef.bounds.min.y;
        targetRangeRef.enabled = false;

        firstTime = false;
        SetTarget();
    }

    // Update is called once per frame
    public override void ResetGame()
    {
        //HasLost = false;
        //HasWon = false;
        //yield return new WaitForSeconds(GameSelect.GameCloseTime-0.1f);
        
        //StartCoroutine(DoResetGame());
    }

    IEnumerator DoResetGame()
    {
        yield return null;
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
        GameSelectManager.QuitGame();
        yield return null;
    }
    public void SetTarget()
    {
        targetRangeSize = Random.Range(1, 0.8f);
        targetPos = Random.Range(targetMinY,targetMaxY);
        TargetRange.transform.position = new Vector3(TargetRange.transform.position.x, targetPos, TargetRange.transform.position.z);
        TargetRange.transform.localScale = new Vector3(TargetRange.transform.localScale.x, targetRangeSize);

        targetMarker.transform.position = TargetRange.transform.position;
    }
    public void SetAngry()
    {
        glassObj.isRotating = true;
        drinker.sprite = drinkerAngry;
        SetLose();
    }
}