using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsManager : MiniGameManager
{
    static MonoBehaviour selfInstance;
    public GameObject humanPrefab;
    public Transform humanPosRef;
    static GameObject HumanObject;
    public static GameObject DartsHolder;
    public static GameObject DebrisHolder;

    //public static GameObject WinObject;
    //public static GameSelect GameSelectManager;

    public override void StartGame()
    {
        Init();
        selfInstance = this;
        
        HumanObject = Instantiate(humanPrefab, humanPosRef.transform.position,transform.rotation);
        HumanObject.transform.SetParent(this.transform, true);
        
        DartsHolder = Instantiate(new GameObject("DartsHolder"), transform.position, transform.rotation);
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
