using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RitualNight
{
    namespace PartyGames
    {
        public class DVDTaskBehavior : MonoBehaviour//TaskBehavior
        {
            public bool HasWon;
            public PlayerController_DV PlayerController;

            [Header("MiniManager")]
            public DVDMiniManager DVMM;

            public void Open() //override 
            {
                //PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                PlayerController.StartGame();
                //HasWon = false;
                //base.StartOpen();
            }
            private IEnumerator DoFinishTask()
            {
                //SetPartyGameResult(true);
                yield return new WaitForSeconds(2);
                //SetStateClosing();
                StartClose();
            }

            public void StartClose() //override 
            {
                //PlayerController.CloseGame();
                //PlayerController.gameObject.SetActive(false);

                //base.StartClose();
            }

            public void WinCheck()
            {
                HasWon = true;
                DVMM.SetWin();
            }
        }
    }
}