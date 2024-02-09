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
            public TvController_DV tvController;

            public int currentScore;
            public int targetScore;
            public int potentialScore;

            //public SpriteRenderer[] barFillArr;

            [SerializeField] private MeshRenderer[] barFillRendererArr;
            [SerializeField] private Texture[] barFillTexArr;
            [SerializeField] private Texture[] barFillTransTexArr;
            private int _barFillLength;

            [SerializeField] private float scoreBlinkSpeed;

            [Header("Crowd")]
            [SerializeField] private Sprite[] crowdHappySpriteArr;
            [SerializeField] private Sprite[] crowdNormalSpriteArr;
            [SerializeField] private SpriteRenderer[] crowdSprRendArr;
            [SerializeField] private ObjectJumpy_BL[] crowdJumpyArr;

            [Header("MiniManager")]
            public DVDMiniManager DVMM;

            public void Open() //override 
            {
                //PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                for(int i = 0; i< crowdSprRendArr.Length;i++)
                {
                    crowdSprRendArr[i].sprite = crowdNormalSpriteArr[i];
                    crowdJumpyArr[i] = crowdSprRendArr[i].gameObject.GetComponent<ObjectJumpy_BL>();
                }
                PlayerController.StartGame();
                //HasWon = false;
                //base.StartOpen();
            }
            private IEnumerator DoFinishTask()
            {
                //SetPartyGameResult(true);
                yield return new WaitForSeconds(3.5f);
                //SetStateClosing();
                StartClose();
            }
            private IEnumerator DoFinishTaskFast()
            {
                //SetPartyGameResult(true);
                yield return new WaitForSeconds(1f);
                //SetStateClosing();
                StartClose();
            }
            private void Awake()
            {
                _barFillLength = barFillRendererArr.Length;
            }
            private void FixedUpdate()
            {
                if (tvController.BounceCount > 0)///
                {
                    for (int i = currentScore; i < Mathf.Clamp(currentScore+tvController.BounceCount,0,_barFillLength); i++)
                    {
                        barFillRendererArr[i].enabled = true;
                        if (Mathf.Sin(Time.time * scoreBlinkSpeed) > 0) //Could also use the tick appoarch to make consistent
                        {
                            barFillRendererArr[i].material.mainTexture = barFillTransTexArr[i];
                        }
                        else
                        {
                            barFillRendererArr[i].material.mainTexture = barFillTexArr[i];
                        }
                    }
                }
            }
            public void ResetPotentialScore()
            {
                for (int i = currentScore; i < _barFillLength; i++)
                {
                    barFillRendererArr[i].enabled = false;
                }
            }
            public void IncreaseScore(int _bounceAmount)
            {
                if (_bounceAmount == 0)
                {
                    Debug.LogError("0 bounce score");
                    PlayerController.CreateNewLogo(1);
                    return;
                }
                tvController.DisableAllKnobs();
                tvController.TurnAllSidesBlue();

                for (int i = currentScore; i < currentScore + _bounceAmount; i++)
                {
                    if (i == _barFillLength)// if it goes over score limit...redundant???????
                    {
                        currentScore += _bounceAmount;
                        if (!WinCheck())
                        {
                            PlayerController.CreateNewLogo(_bounceAmount);
                        }
                        return;
                    }
                    barFillRendererArr[i].material.mainTexture = barFillTexArr[i];
                    barFillRendererArr[i].enabled = true;
                }
                currentScore += _bounceAmount;
                if (!WinCheck())
                {
                    PlayerController.CreateNewLogo(_bounceAmount);
                }
            }
            private void ResetScore() 
            {
                currentScore = 0;
                potentialScore = 0;
                foreach (MeshRenderer _meshRend in barFillRendererArr)
                {
                    _meshRend.enabled = false;
                }
            }
            public void StartClose() //override 
            {
                StopAllCoroutines();
                PlayerController.CloseGame();
                PlayerController.gameObject.SetActive(false);

                ResetScore();

                //base.StartClose();
            }

            public bool WinCheck()
            {
                if (currentScore > targetScore)
                {
                    tvController.ShowWinScreen();
                    for (int i = 0; i < crowdSprRendArr.Length; i++)
                    {
                        crowdSprRendArr[i].sprite = crowdHappySpriteArr[i];
                        crowdJumpyArr[i].Jump();
                    }

                    HasWon = true;
                    StartCoroutine(DoFinishTaskFast());

                    DVMM.SetWin();
                    return true;
                }
                else if (currentScore == targetScore)
                {
                    tvController.ShowWinScreen();
                    for (int i = 0; i < crowdSprRendArr.Length; i++)
                    {
                        crowdSprRendArr[i].sprite = crowdHappySpriteArr[i];
                        crowdJumpyArr[i].Jump();
                    }
                    HasWon = true;
                    StartCoroutine(DoFinishTask());

                    DVMM.SetWin();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}