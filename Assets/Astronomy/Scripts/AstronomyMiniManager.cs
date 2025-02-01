using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RitualNight.PartyGames;

public class AstronomyMiniManager : MiniGameManager
{
    public AstronomyTaskBehavior ASTB;
    public bool HasWon;
    public bool TestingWithoutArcade;
    // Start is called before the first frame update
    private void Start()
    {
        if (TestingWithoutArcade)
        {
            ASTB.StartOpen();
        }
    }
    public override void StartGame() //StartOpen
    {
        Init();
        ASTB.StartOpen();
    }
    public override void ResetGame() //startclose
    {
        ASTB.StartClose();
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
