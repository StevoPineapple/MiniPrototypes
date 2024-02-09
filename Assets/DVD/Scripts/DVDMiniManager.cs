using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RitualNight.PartyGames;

public class DVDMiniManager : MiniGameManager
{
    public DVDTaskBehavior DVTB;
    public PlayerController_DV PlayerController;
    public bool HasWon;
    public bool TestingWithoutArcade;
    // Start is called before the first frame update
    private void Start()
    {
        if (TestingWithoutArcade)
        {
            DVTB.StartOpen();
        }
    }
    public override void StartGame() //StartOpen
    {
        Init(); 
        PlayerController.gameObject.SetActive(true);
        DVTB.StartOpen();
    }
    public override void ResetGame() //startclose
    {
        DVTB.StartClose();
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
