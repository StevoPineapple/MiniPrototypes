using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RitualNight.PartyGames;

public class RingBellMiniManager : MiniGameManager
{
    public RingBellTaskBehavior RBTB;
    public bool HasWon;
    public bool TestingWithoutArcade;
    // Start is called before the first frame update
    private void Start()
    {
        if (TestingWithoutArcade)
        {
            RBTB.StartOpen();
        }
    }
    public override void StartGame() //StartOpen
    {
        Init();
        RBTB.StartOpen();
    }
    public override void ResetGame() //startclose
    {
        RBTB.StartClose();
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
        yield return new WaitForEndOfFrame();
        GameSelectManager.QuitGame();
    }
}
