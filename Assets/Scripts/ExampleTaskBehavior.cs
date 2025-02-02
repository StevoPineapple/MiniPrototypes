using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class ExampleTaskBehavior : MonoBehaviour//TaskBehavior
        {
            public bool HasWon;
            
            [Header ("MiniManager")]
            public ExampleMiniManager EXMM;

            public void Open() //override 
            {
                //PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                HasWon = false;
                //PlayerController.StartSetUp();
                //base.StartOpen();
            }
            private IEnumerator DoFinishTask()
            {
                //SetPartyGameResult(true);
                yield return new WaitForSeconds(2);
                EXMM.SetWin();
                //SetStateClosing();
            }
                            
            public void StartClose() //override 
            {
                StopAllCoroutines();
                //PlayerController.CloseGame();
                //PlayerController.gameObject.SetActive(false);

                //base.StartClose();
            }

            public void WinCheck()
            {
                HasWon = true;
                StartCoroutine(DoFinishTask());
            }
        }
    }
}