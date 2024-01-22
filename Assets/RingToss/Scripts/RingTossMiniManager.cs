using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTossMiniManager : MiniGameManager
{
    public RingTossTaskBehavior RTTB;
    public bool HasWon;
    public bool Testing;
    // Start is called before the first frame update
    private void Start()
    {
        if (Testing)
        {
            RTTB.StartOpen();
        }
    }
    public override void StartGame() //StartOpen
    {
        Init();
        RTTB.StartOpen();
    }
    public override void ResetGame() //startclose
    {
        RTTB.StartClose();
    }

    public override void SetWin()
    {
        StartCoroutine(DoSetWin());
    }

    // Update is called once per frame
    IEnumerator DoSetWin()
    {
        HasWon = true;
        WinObject.SetActive(true);
        yield return new WaitForSeconds(GameSelect.GameCloseTime);
        GameSelectManager.QuitGame();
    }
}
