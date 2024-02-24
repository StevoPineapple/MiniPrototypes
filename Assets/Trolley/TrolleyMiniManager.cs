using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RitualNight.PartyGames;

public class TrolleyMiniManager : MiniGameManager
{
    public TrolleyTaskBehavior TRTB;
    public bool HasWon;
    public bool TestingWithoutArcade;
    // Start is called before the first frame update
    private void Start()
    {
        if (TestingWithoutArcade)
        {
            TRTB.StartOpen();
        }
    }
    public override void StartGame() //StartOpen
    {
        Init();
        TRTB.StartOpen();
    }
    public override void ResetGame() //startclose
    {
        TRTB.StartClose();
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
