using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RitualNight
{
    namespace PartyGames
    {
        public class WhackAMoleTaskBehavior : MonoBehaviour//TaskBehavior
        {
            public bool HasWon;
            [SerializeField] private MoleSpawnControl_WKM spawnControl;
            [SerializeField] private PlayerController_WKM PlayerController;
            [Header("ModeTune")]
            [Tooltip("if doesn't hit mode0chance, go for mode 1 or mode 2")]
            [SerializeField] private float mode0Chance;
            [SerializeField] private float mode1Chance;

            [Header("Score")]
            [SerializeField] private GameObject[] fiveLightsArr;
            [SerializeField] private GameObject[] sixLightsArr;
            [SerializeField] private GameObject[] nineLightsArr;
            [SerializeField] private GameObject fiveLightsBG;
            [SerializeField] private GameObject sixLightsBG;
            [SerializeField] private GameObject nineLightsBG;
            private GameObject[] _currLightsArr;

            private int _currPoint;
            private int _targetPoint;

            [Header ("MiniManager")]
            public WhackAMoleMiniManager WKMMM;

            public void Open() //override 
            {
                PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                HasWon = false;
                ResetScoreDisplay();
                spawnControl.StartSpawning(ChooseMode());
                PlayerController.gameObject.SetActive(true);
                PlayerController.StartGame();
                //base.StartOpen();
            }
            private void ResetScoreDisplay()
            {
                fiveLightsBG.SetActive(false);
                sixLightsBG.SetActive(false);
                nineLightsBG.SetActive(false);
                foreach (GameObject obj in fiveLightsArr)
                {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in sixLightsArr)
                {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in nineLightsArr)
                {
                    obj.SetActive(false);
                }
            }
            private int ChooseMode()
            {
                _currPoint = 0;
                if (Random.Range(0f, 1f) < mode0Chance)
                {
                    //normal mode
                    _targetPoint = 6;
                    _currLightsArr = sixLightsArr;
                    sixLightsBG.SetActive(true);
                    return 0;
                }
                else if (Random.Range(0f, 1f) < mode1Chance)
                {
                    //everything but 1 mode
                    _targetPoint = 5;
                    _currLightsArr = fiveLightsArr;
                    fiveLightsBG.SetActive(true);
                    return 1;
                }
                else
                {
                    //order mode
                    _targetPoint = 9;
                    _currLightsArr = nineLightsArr;
                    nineLightsBG.SetActive(true);
                    return 2;
                }

            }
            private IEnumerator DoFinishTask()
            {
                spawnControl.StopCoroutine();
                //SetPartyGameResult(true);
                yield return new WaitForSeconds(1);
                WKMMM.SetWin();
                //SetStateClosing();
            }
                            
            public void StartClose() //override 
            {
                spawnControl.ClearMoleParent();
                StopAllCoroutines();
                PlayerController.CloseGame();
                PlayerController.gameObject.SetActive(false);

                //base.StartClose();
            }
            public void AddPoint(int amount, MoleBehavior_WKM lastMole)
            {
                for (int i = 0; i < amount; i++)
                {
                    _currLightsArr[Mathf.Clamp(_currPoint+i,0,_currLightsArr.Length)].SetActive(true);
                }
                _currPoint+=amount;
                WinCheck(lastMole);
            }
            public void WinCheck(MoleBehavior_WKM lastMole)
            {
                if (_currPoint >= _targetPoint)
                {
                    HasWon = true;
                    spawnControl.AllMolesGoDown(lastMole);
                    StartCoroutine(DoFinishTask());
                }
            }
        }
    }
}