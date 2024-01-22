using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

        public class UIManager_RT : MonoBehaviour
        {
            public TextMeshProUGUI ScoreText;
            public TextMeshProUGUI TotalScoreText;
            public RingTossTaskBehavior RTManager;
            [SerializeField] private int ruleCount;
            public TextMeshProUGUI NormalRuleText;
            public TextMeshProUGUI RedRuleText;
            public TextMeshProUGUI SpRuleText;
            public SpriteRenderer SpSprRend;
            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                ScoreText.text = RTManager.CurrentScore.ToString();
            }
            public void SetScoreTarget(int point)
            {
                TotalScoreText.text = "/" + point;
            }
            public void AddNormalScoreRule(int point)
            {
                if (point > 0)
                {
                    NormalRuleText.text = "+" + point;
                }
                else
                {
                    NormalRuleText.text = point.ToString();
                }
            }
            public void AddRedScoreRule(int point)
            {
                if (point > 0)
                {
                    RedRuleText.text = "+" + point;
                }
                else
                {
                    RedRuleText.text = point.ToString();
                }
            }
            public void AddScoreRuleSpecial(Sprite sprite, int point)
            {
                SpSprRend.sprite = sprite;
                if (point > 0)
                {
                    SpRuleText.text = "+" + point;
                }
                else
                {
                    SpRuleText.text = point.ToString();
                }
            }
        }