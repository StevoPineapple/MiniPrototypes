using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RitualNight.PartyGames;

public class BlindBagMiniManager : MiniGameManager
{
    public BlindBagTaskBehavior BLTB;
    public bool HasWon;
    public bool TestingWithoutArcade;
    // Start is called before the first frame update
    private void Start()
    {
        if (TestingWithoutArcade)
        {
            BLTB.gameObject.SetActive(true);
            BLTB.StartOpen();
        }
    }
    public override void StartGame() //StartOpen
    {
        Init();
        BLTB.StartOpen();
    }
    public override void ResetGame() //startclose
    {
        BLTB.StartClose();
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
