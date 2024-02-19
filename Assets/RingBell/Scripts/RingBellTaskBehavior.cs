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
            [Header("Components")]
            public LaunchBar_RB LaunchBar;
            public ParallaxManager_RB ParaManager;
            public bool HasWon;
            
            [Header ("OkBell")]
            [SerializeField] private Transform okBellTrans;
            [SerializeField] private Transform SlideyTrans;
            [SerializeField] private GameObject eggCry;
            [SerializeField] private AnimationCurve returnTimeCurve;

            [Header ("MiniManager")]
            public RingBellMiniManager RBMM;

            public void Open() //override 
            {
                //PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                ResetGame();
                ParaManager.StartGame();
                LaunchBar.StartGame();
                //PlayerController.StartSetUp();
                //HasWon = false;
                //base.StartOpen();
            }
            private IEnumerator DoFinishTask(float _heightScore)
            {
                //SetPartyGameResult(true);
                yield return new WaitForSeconds(returnTimeCurve.Evaluate(_heightScore));
                //SetStateClosing();
                StartClose();
                HasWon = true;
                RBMM.SetWin();
            }

            public float GetReturnTime(float _heightScore)
            {
                return returnTimeCurve.Evaluate(_heightScore);
            }
            public void StartClose() //override 
            {
                LaunchBar.ResetGame();
                ParaManager.ResetGame();
                ResetGame();
                //PlayerController.CloseGame();
                //PlayerController.gameObject.SetActive(false);

                //base.StartClose();
            }
            public void WinCheck(bool _isOK, float _heightScore)
            {
                if (_isOK)
                {
                    StartCoroutine(DoFinishTask(_heightScore));
                    //set win with time ratio
                }
                else
                {
                    eggCry.SetActive(true);
                    StartCoroutine(DoResetGame());
                }
            }
            IEnumerator DoResetGame()
            {
                yield return new WaitForSeconds(1.5f);

                ResetGame();
                ParaManager.StartGame();
                LaunchBar.StartGame();
            }
            private void ResetGame()
            {
                StopAllCoroutines();
                eggCry.SetActive(false);
            }
        }
    }
}