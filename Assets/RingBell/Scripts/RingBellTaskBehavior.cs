using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RitualNight
{
    namespace PartyGames
    {
        public class RingBellTaskBehavior : MonoBehaviour//TaskBehavior
        {
            public bool HasWon;
            
            [Header ("MiniManager")]
            public RingBellMiniManager RBMM;

            public void Open() //override 
            {
                //PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                //PlayerController.StartSetUp();
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
                RBMM.SetWin();
            }
        }
    }
}