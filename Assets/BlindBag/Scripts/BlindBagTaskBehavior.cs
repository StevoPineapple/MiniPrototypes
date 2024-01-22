using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RitualNight
{
    namespace PartyGames
    {
        public class BlindBagTaskBehavior : MonoBehaviour//TaskBehavior
        {
            public bool HasWon;
            public PlayerController_BL PlayerController;
            public BlindBagMiniManager BLMM;
            private GameObject PrizePattern;

            [Header("PrizePattern")]
            [SerializeField] private GameObject[] PrizePatternArr;
            [SerializeField] private Transform BagTransform;

            [SerializeField] private ExpressionController_BL Expression;

            public void Open() //override 
            {
                PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                PlayerController.gameObject.SetActive(true);
                PlayerController.StartGame();
                RandomizePrizes();
                //HasWon = false;
                //base.StartOpen();
            }

            private void RandomizePrizes()
            {
                int _prizePatternIndex = Random.Range(0, PrizePatternArr.Length);
                PrizePattern = Instantiate(PrizePatternArr[_prizePatternIndex], BagTransform);

                //Make 2 gross
                int _grossIndex = Random.Range(0, PrizePattern.transform.childCount);
                PrizePattern.transform.GetChild(_grossIndex).tag = "Gross_BL";
                _grossIndex = Random.Range(0, PrizePattern.transform.childCount);
                PrizePattern.transform.GetChild(_grossIndex).tag = "Gross_BL";

                //Make 1 prize
                int _prizeIndex = Random.Range(0, PrizePattern.transform.childCount);
                Transform PrizeChild = PrizePattern.transform.GetChild(_prizeIndex);
                PrizeChild.gameObject.tag = "Prize_BL";
                PlayerController.PrizeTransform = PrizeChild;
            }

            public void SetWin()
            {
                HasWon = true;
                BLMM.SetWin();
                //StartCoroutine(DoFinishTask());
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
                HasWon = false;
                Expression.gameObject.SetActive(false);
                Destroy(PrizePattern);
                PlayerController.CloseGame();
                PlayerController.gameObject.SetActive(false);
                //base.StartClose();
            }
        }
    }
}