using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RitualNight.PartyGames;

public class ExampleMiniManager : MiniGameManager
{
    public ExampleTaskBehavior EXTB;
    public bool HasWon;
    public bool TestingWithoutArcade;
    // Start is called before the first frame update
    private void Start()
    {
        if (TestingWithoutArcade)
        {
            EXTB.StartOpen();
        }
    }
    public override void StartGame() //StartOpen
    {
        Init();
        EXTB.StartOpen();
    }
    public override void ResetGame() //startclose
    {
        EXTB.StartClose();
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
