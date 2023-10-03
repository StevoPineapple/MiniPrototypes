using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameManager : MonoBehaviour
{
    public static GameSelect GameSelectManager;
    public GameObject WinObject;
    // Start is called before the first frame update
    public void Init()
    {
        GameSelectManager = GameObject.FindWithTag("GameSelectManager").GetComponent<GameSelect>();
        WinObject = transform.Find("Win").gameObject;
        WinObject.SetActive(false);
    }
    public abstract void StartGame();
    public abstract void SetWin();
    IEnumerator SetWinIE(){ yield return null;}
    public abstract void ResetGame();
    IEnumerator DestroyObjects(){yield return null;}
}
