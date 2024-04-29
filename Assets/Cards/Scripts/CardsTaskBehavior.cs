using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class CardsTaskBehavior : MonoBehaviour//TaskBehavior
        {
            public bool HasWon;
            [SerializeField] private PlayerController_CRD playerController;

            [Header("MiniManager")]
            public CardsMiniManager CRDMM;

            [Header("Cards")]
            [SerializeField] private GameObject memoryBG;
            [SerializeField] private CardBehavior_CRD[] cardArr;
            [SerializeField] private GameObject cardParent;
            [SerializeField] private Transform cardParticleParent;
            [SerializeField] private GameObject cardParticlePrefab;
            private int _chosenSetIndex;

            [Header("Select")]
            [SerializeField] private int candlePointsToWin;
            private int _currentCandlePoint;
            [SerializeField] private GameObject[] candleFlameArr;
            [SerializeField] private PartyGameObjectJumpy[] candleFlameJumpArr;

            
            
            private List<Sprite> tableCardSprList = new List<Sprite>();
            [SerializeField] private float revealWaitTime;
            [SerializeField] private float wrongRevealWaitTime;

            [Header("CardSets")]
            [SerializeField] private CardSet[] cardSetsArr;
            [System.Serializable]
            public class CardSet
            {
                public string name;
                public Sprite[] FaceSprArr; //init in inspector
                public Sprite BackSpr;
            }

            [Header("Cat")]
            [SerializeField] private SpriteRenderer catSprRend;
            [SerializeField] private Sprite catNormalSpr;
            [SerializeField] private Sprite catHappySpr;
            [SerializeField] private Sprite catAngrySpr;
            [SerializeField] private Sprite tickSpr;
            [SerializeField] private Sprite crossSpr;
            [SerializeField] private SpriteRenderer sprRendInBall;
            private Sprite _targetCardSpr;

            [Header("Lose/Win")]
            [SerializeField] private GameObject winParticle;
            [SerializeField] private PartyGameObjectJumpy loseCrossJumpy;
            [SerializeField] private float loseWaitTime;
            [SerializeField] private GameObject loseHintUI;

            [Header("MISC")]
            [SerializeField] private Sprite eyeOpenSpr;
            [SerializeField] private Sprite eyeClosedSpr;
            [SerializeField] private SpriteRenderer eyeSprRend;

            int ChooseSet()// don't choose set if lose
            {
                return Random.Range(0, cardSetsArr.Length);
            }

            void ChooseCards()
            {
                if (_chosenSetIndex == -1)
                { _chosenSetIndex = ChooseSet(); }

                //if(number chance goes here)
                int _cardNumber = cardArr.Length;
                if (_cardNumber < 5)
                {
                    throw new System.Exception("less than 5 in cardArr " + _chosenSetIndex);
                }

                //create index list
                List<Sprite> _faceList = new List<Sprite>();
                for (int i = 0; i < cardSetsArr[_chosenSetIndex].FaceSprArr.Length; i++)
                {
                    _faceList.Add(cardSetsArr[_chosenSetIndex].FaceSprArr[i]);
                }

                for (int i = 0; i < _cardNumber; i++)
                {
                    cardArr[i].SetCardBack(cardSetsArr[_chosenSetIndex].BackSpr);
                    int _chosenIndex = Random.Range(0, _faceList.Count);
                    cardArr[i].FaceSpr = _faceList[_chosenIndex];
                    _faceList.RemoveAt(_chosenIndex);
                    cardArr[i].FlipToFront();
                    tableCardSprList.Add(cardArr[i].FaceSpr);
                }
            }
            void FlipAllCardsBack()
            {
                foreach (CardBehavior_CRD _card in cardArr)
                {
                    _card.FlipToBack();
                }
            }
            public void EndMemoryPhase()
            {
                StartCoroutine(DoEndMemoryPhase());
            }
            IEnumerator DoEndMemoryPhase()
            {
                FlipAllCardsBack();
                eyeSprRend.sprite = eyeClosedSpr;
                yield return new WaitForSeconds(0.16f);//match 10f in card animation
                memoryBG.SetActive(false);
                cardParent.GetComponent<Animator>().Play("CardsMove");
                yield return new WaitForSeconds(0.16f);

                playerController.ColliderEnable(true);
                playerController.SetCanSelect(true);

                ChooseTargetCard();

            }
            void ChooseTargetCard()
            {
                int _selectedIndex = Random.Range(0, tableCardSprList.Count);
                _targetCardSpr = tableCardSprList[_selectedIndex];
                tableCardSprList.RemoveAt(_selectedIndex);
                sprRendInBall.sprite = _targetCardSpr;
            }
            public void RevealCard(CardBehavior_CRD _revealedCard)
            {
                if (_revealedCard.FaceSpr == _targetCardSpr)
                {
                    StartCoroutine(DoReveal(true, _revealedCard.gameObject));
                }
                else
                {
                    StartCoroutine(DoReveal(false, _revealedCard.gameObject));
                    tableCardSprList.Remove(_revealedCard.FaceSpr);
                }
            }
            IEnumerator DoReveal(bool _isCorrect, GameObject _revealedCard)//FLip card in handled in player controller
            {
                playerController.ColliderEnable(false);
                if (_isCorrect)//shoot particle
                {
                    sprRendInBall.sprite = tickSpr;
                    catSprRend.sprite = catHappySpr;
                    CandleParticle_CRD part = Instantiate(cardParticlePrefab, _revealedCard.transform.position, Quaternion.identity, cardParticleParent).GetComponent<CandleParticle_CRD>();
                    part.CRDManager = this;
                    part.FlyToCandle(candleFlameArr[_currentCandlePoint]);
                    yield return null;
                    //This ends here until the candle particle hits the candle
                }
                else
                {
                    sprRendInBall.sprite = crossSpr;
                    catSprRend.sprite = catAngrySpr;
                    yield return new WaitForSeconds(wrongRevealWaitTime);
                    catSprRend.sprite = catNormalSpr;
                    playerController.ColliderEnable(true);
                    //playsound
                    sprRendInBall.sprite = _targetCardSpr;
                    CheckWinnable();
                }
            }
            public void LightCandle()
            {                    //playsound
                catSprRend.sprite = catNormalSpr;
                playerController.ColliderEnable(true);
                candleFlameArr[_currentCandlePoint].SetActive(true);
                candleFlameJumpArr[_currentCandlePoint].Jump();
                _currentCandlePoint++;
                WinCheck();
                if (tableCardSprList.Count > 0)
                {
                    ChooseTargetCard();
                }
            }
            void CheckWinnable()
            {
                if (_currentCandlePoint + tableCardSprList.Count + 1 >= candlePointsToWin)
                {
                    //can win, ok
                    return;
                }
                else
                {
                    //lose
                    StartCoroutine(DoLoseGame());
                }
            }
            public void Open() //override 
            {
                //PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                winParticle.SetActive(false);
                loseHintUI.SetActive(false);
                loseCrossJumpy.gameObject.SetActive(false);   
                sprRendInBall.sprite = null;
                catSprRend.sprite = catNormalSpr;
                eyeSprRend.sprite = eyeOpenSpr;
                foreach(CardBehavior_CRD card in cardArr)
                {
                    card.enabled = true;

                }
                _chosenSetIndex = -1;
                ChooseCards();

                //PlayerController.StartSetUp();
                //HasWon = false;
                //base.StartOpen();
            }
            private IEnumerator DoFinishTask()
            {
                //SetPartyGameResult(true);
                yield return new WaitForSeconds(1);
                CRDMM.SetWin();
                //SetStateClosing();
            }
                            
            public void StartClose() //override 
            {
                ResetGame();

                //PlayerController.CloseGame();
                //PlayerController.gameObject.SetActive(false);

                //base.StartClose();
            }

            public void WinCheck()
            {
                if (_currentCandlePoint == candlePointsToWin)
                {
                    playerController.ColliderEnable(false);
                    foreach (PartyGameObjectJumpy fireJumpy in candleFlameJumpArr)
                    {
                        fireJumpy.Jump();
                    }
                    winParticle.SetActive(true);
                    HasWon = true;
                    StartCoroutine(DoFinishTask());
                }
            }
            IEnumerator DoLoseGame()
            {
                playerController.ColliderEnable(false);
                catSprRend.sprite = catAngrySpr;
                loseCrossJumpy.gameObject.SetActive (true);
                loseHintUI.SetActive(true);
                loseCrossJumpy.Jump();
                yield return new WaitForSeconds(loseWaitTime);
                foreach (CardBehavior_CRD card in cardArr)
                {
                    card.FlipToBack();
                }
                yield return new WaitForSeconds(loseWaitTime);
                loseCrossJumpy.gameObject.SetActive(false);
                ResetGame();
                ChooseCards();
            }
            void ResetGame()
            {
                winParticle.SetActive(false);
                loseHintUI.SetActive(false);
                loseCrossJumpy.gameObject.SetActive(false);
                _currentCandlePoint = 0;

                sprRendInBall.sprite = null;
                catSprRend.sprite = catNormalSpr;

                tableCardSprList.Clear();

                foreach (GameObject obj in candleFlameArr)
                {
                    obj.SetActive(false);
                }

                foreach (Transform t in cardParticleParent)
                {
                    Destroy(t.gameObject);
                }

                foreach (CardBehavior_CRD card in cardArr)
                {
                    card.enabled = true;
                    card.SetEdgeVisible(false);
                    card.EnableSelf();
                }
                cardParent.GetComponent<Animator>().Play("CardsReset");

                memoryBG.SetActive(true);
                eyeSprRend.sprite=eyeOpenSpr;

                StopAllCoroutines();

                playerController.ResetGame();
            }
        }
    }
}