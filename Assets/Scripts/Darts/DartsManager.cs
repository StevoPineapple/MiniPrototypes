using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsManager : MiniGameManager
{
    static MonoBehaviour selfInstance;
    public GameObject[] humanPrefabArr;
    public Transform humanPosRef;
    static GameObject HumanObject;
    public static GameObject DartsHolder; //TODO static shouldn't be used like this, if it changes
    public static GameObject DebrisHolder;

    int humanLevel;

    //public static GameObject WinObject;
    //public static GameSelect GameSelectManager;

    public override void StartGame()
    {
        Init();
        selfInstance = this;

        if(Random.Range(0,1)>0.75f)
            humanLevel = Random.Range(0,humanPrefabArr.Length);
        else
            humanLevel = Random.Range(0, humanPrefabArr.Length-1);

        HumanObject = Instantiate(humanPrefabArr[humanLevel], humanPosRef.transform.position,transform.rotation);
        HumanObject.GetComponent<DartsHumanManager>().SetLevel(humanLevel);
        HumanObject.transform.SetParent(this.transform, true);
        
        DartsHolder = Instantiate(new GameObject("DartsHolder"), transform.position, transform.rotation); //TODO 
        DartsHolder.transform.SetParent(this.transform);

        DebrisHolder = Instantiate(new GameObject("DebrisHolder"), transform.position, transform.rotation);
        DebrisHolder.transform.SetParent(this.transform);

        //Maybe Delete
        //GameSelectManager = GameObject.FindWithTag("GameSelectManager").GetComponent<GameSelect>();
        //WinObject = transform.Find("Win").gameObject;
        //WinObject.SetActive(false);
    }
    public override void SetWin()
    {
        selfInstance.StartCoroutine(SetWinIE());
    }
    IEnumerator SetWinIE()
    {
        WinObject.SetActive(true);
        GameSelectManager.QuitGame();
        yield return null;
    }
    public override void ResetGame()
    {
        selfInstance.StartCoroutine(DestroyObjects());
    }
    static IEnumerator DestroyObjects()
    {
        yield return new WaitForSeconds(GameSelect.GameCloseTime);
        Destroy(HumanObject);
        Destroy(DartsHolder);
        Destroy(DebrisHolder);
    }
}
