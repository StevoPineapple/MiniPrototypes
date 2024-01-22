using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RitualNight
{
    namespace PartyGames
    {
        public class ExpressionController_BL : MonoBehaviour
        {
            [Header ("Egg")]
            [SerializeField] private SpriteRenderer EggSprRend;
            [SerializeField] private ObjectJumpy_BL EggJumpy;
            [SerializeField] private Sprite ShakeLeftSpr;
            [SerializeField] private Sprite ShakeRightSpr;
            [SerializeField] private Sprite CheckingSpr;
            [SerializeField] private Sprite GrossSpr;
            [SerializeField] private Sprite PrizeWinSpr;
            [SerializeField] private int ShakeTickMax;
            private bool _isShaking;
            private bool _isShakingLeft;
            private int _shakeTick;

            [Header("BG")]
            [SerializeField] private SpriteRenderer BGSprRend;
            [SerializeField] private SpriteMask BGSprMask;
            [SerializeField] private Sprite SearchingBG;
            [SerializeField] private Sprite CheckingBG;
            [SerializeField] private Sprite GrossBG;
            [SerializeField] private Sprite PrizeWinBG;

            [Header ("Bubble")]
            [SerializeField] private SpriteRenderer BubbleSprRend;
            [SerializeField] private Sprite BubbleFalseSpr;
            [SerializeField] private Sprite BubbleCheckingSpr;
            [SerializeField] private Sprite BubbleWinSpr;

            [Header("Exclaim")]
            [SerializeField] private SpriteRenderer ExclaimSprRend;
            [SerializeField] private AnimationCurve ExclaimCurve;
            [SerializeField] private float CurveIncrese;
            [SerializeField] private Sprite ExclaimSprite;
            [SerializeField] private Sprite QuestionSprite;

            [Header("Hair")]
            [SerializeField] private SpriteRenderer HairSprRend;
            [SerializeField] private Sprite HairFarSpr;
            [SerializeField] private Sprite HairCloseSpr;

            private float _curvePosition;

            // Start is called before the first frame update
            public void OnStartGame()
            {
                BubbleSprRend.enabled = false;
                BGSprRend.sprite = CheckingBG;
                EggSprRend.enabled = false;
                _curvePosition = 1;
            }

            private void FixedUpdate()
            {
                if (_isShaking)
                {
                    if (_shakeTick > 0)
                    {
                        _shakeTick--;
                    }
                    else
                    {
                        if (_isShakingLeft)
                        {
                            EggSprRend.sprite = ShakeRightSpr;
                            _isShakingLeft = false;
                        }
                        else
                        {
                            EggSprRend.sprite = ShakeLeftSpr;
                            _isShakingLeft = true;
                        }
                        _shakeTick = ShakeTickMax;
                    }
                }
                //if (_isExclaiming)
                //{
                    float _currentSize = Mathf.Clamp(ExclaimCurve.Evaluate(_curvePosition),0f,1f);
                    _curvePosition += CurveIncrese;
                    ExclaimSprRend.transform.localScale = new Vector3(_currentSize, _currentSize, _currentSize);
                //}
            }

            public void HairClose()
            {
                HairSprRend.sprite = HairCloseSpr;
            }

            public void HairFar()
            {
                HairSprRend.sprite = HairFarSpr;
            }

            public void Exclaim()
            {
                //_isExclaiming = true;
                ExclaimSprRend.sprite = ExclaimSprite;
                _curvePosition = 0;
            }

            public void Question()
            {
                //_isExclaiming = true;
                ExclaimSprRend.sprite = QuestionSprite;
                _curvePosition = 0;
            }

            public void EggSearching()
            {
                StopShaking();
                BubbleSprRend.enabled = false;
                EggSprRend.sprite = CheckingSpr;
                //BGSprRend.sprite = SearchingBG;
            }
            public void EggSearchingBG()
            {
                BGSprRend.sprite = SearchingBG;
            }
            public void EggCheckingBG()
            {
                BGSprRend.sprite = CheckingBG;
            }
            public void EggChecking()
            {
                EggJumpy.Jump();
                StopShaking();
                BubbleSprRend.enabled = true;
                BubbleSprRend.sprite = BubbleCheckingSpr;
                EggSprRend.sprite = CheckingSpr;
                BGSprRend.sprite = CheckingBG;
            }
            public void EggShaking()
            {
                EggJumpy.Jump();
                StopShaking();
                _isShaking = true;
                BubbleSprRend.enabled = true;
                BubbleSprRend.sprite = BubbleFalseSpr;
            }
            public void EggGross()
            {
                EggJumpy.Jump();
                StopShaking();
                BubbleSprRend.enabled = false;
                EggSprRend.sprite = GrossSpr;
                BGSprRend.sprite = GrossBG;
            }
            public void EggPrizeWin()
            {
                StopShaking();
                EggJumpy.Jump();
                BubbleSprRend.enabled = true;
                BubbleSprRend.sprite = BubbleWinSpr;
                EggSprRend.sprite = PrizeWinSpr;
                BGSprRend.sprite = PrizeWinBG;
            }
            private void StopShaking()
            {
                _isShaking = false;
                _isShakingLeft = true;
                _shakeTick = ShakeTickMax;
            }
        }
    }
}