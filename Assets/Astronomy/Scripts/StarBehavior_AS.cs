using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class StarBehavior_AS : MonoBehaviour
        {
            public bool IsGood;
            public bool HasClicked { private set; get; }
            public bool IsConnected { private set; get; }
            [SerializeField] private SpriteRenderer starSprRend;
            [SerializeField] private float starSizeRandomAdj;
            [SerializeField] private PartyGameEnlarge sprEnlarge;

            [SerializeField] private Sprite starClickedSpr;
            [SerializeField] private Color wrongClickedColor;
            [SerializeField] private float alphaOffset;
            public bool IsCelebrateRotation;

            [Header("Hover")]
            [SerializeField] private Color hoverColor;
            [SerializeField] private Color nonHoverColor;
            [SerializeField] private Vector3 hoverRotation;
            private Vector3 _rotationTarget;
            [SerializeField] private float hoverLerpSpeed;
            private bool _isHovering;

            [Header("Connections")]
            [SerializeField] private List<StarBehavior_AS> neighborStarsList = new List<StarBehavior_AS>();
            [SerializeField] private GameObject starLinePrefab;
            private List<LineRenderer> _lineRendList = new List<LineRenderer>();

            [Header("Fadeout")]
            private bool isFading;
            [SerializeField] private float fadeoutBaseAmount;
            private float _fadeoutAmount;
            [SerializeField] private float fadeoutRandomOffset;
            [SerializeField] private float fadeoutAlpha;

            void Start()
            {
                nonHoverColor = new Color(nonHoverColor.r, nonHoverColor.g, nonHoverColor.b, nonHoverColor.a + Random.Range(-alphaOffset, alphaOffset));
                starSprRend.color = nonHoverColor;
                float _randomSize = 1; 
                if (IsGood)
                {
                    _randomSize /= Mathf.Abs(transform.parent.parent.localScale.x);//account scale of formation parent
                    _randomSize /= Mathf.Abs(transform.parent.parent.parent.parent.localScale.x);//account scale for photo ref stretch
                    //print("tppl" + transform.parent.parent.localScale.x);
                }
                else
                {
                    _fadeoutAmount = fadeoutBaseAmount + Random.Range(-fadeoutRandomOffset, fadeoutRandomOffset);
                    AstronomyTaskBehavior.onEasyModeEvt += StartFadeOut;
                }
                _randomSize += Random.Range(-starSizeRandomAdj,starSizeRandomAdj)* Mathf.Abs(transform.parent.parent.localScale.x);
                //print(_randomSize);
                starSprRend.transform.localScale = new Vector3(_randomSize, _randomSize, _randomSize);
                sprEnlarge.SetOriginalSize();
                //hoverRotationQuaternion = Quaternion.Euler(hoverRotation);
                IsCelebrateRotation = false;
            }

            [Tooltip ("Checks and completes any missing ref in neighbor star array")]
            public void CompleteNeighbors()
            {
                foreach (StarBehavior_AS _star in neighborStarsList)
                {
                    _star.CheckAddNeighbor(this);
                }
            }
            [Tooltip ("Other stars can use this to add themselves to neighbor arr")]
            public void CheckAddNeighbor(StarBehavior_AS _otherStar)
            {
                if (!neighborStarsList.Contains(_otherStar))
                {
                    neighborStarsList.Add(_otherStar);
                }
            }
            [Tooltip ("Create a linerenderer for every neighbor")]
            public void CreateLineRenderer()
            {
                foreach (StarBehavior_AS star in neighborStarsList)
                {
                    LineRenderer _line = Instantiate(starLinePrefab, transform.position, Quaternion.identity, transform).GetComponent<LineRenderer>();
                    _line.useWorldSpace = true;
                    _lineRendList.Add(_line);
                }
            }
            void Update()
            {
                if (IsCelebrateRotation)
                {
                    starSprRend.color = Color.white;
                    SmoothRotate(new Vector3(0, 0, 180));
                    return;
                }
                else if (HasClicked)
                {
                    SmoothRotate(Vector3.zero);
                    return;
                }
                else
                {
                    if (_isHovering)
                    {
                        RotateHoverState();
                    }
                    else
                    {
                        RotateBackToNormal();
                    }
                }
            }
            void RotateHoverState()
            {
                if (!isFading)
                {
                    starSprRend.color = hoverColor;
                }
                SmoothRotate(hoverRotation);
            }
            void RotateBackToNormal()
            {
                if (!isFading)
                {
                    starSprRend.color = nonHoverColor;
                }
                SmoothRotate(Vector3.zero);
            }
            void SmoothRotate(Vector3 rotAngle)
            {
                _rotationTarget = Vector3.Lerp(_rotationTarget, rotAngle, hoverLerpSpeed * Time.deltaTime);
                starSprRend.transform.rotation = Quaternion.Euler(_rotationTarget);
            }
            public void OnHover()
            {
                _isHovering = true;
                sprEnlarge.Enlarge();
            }
            public void OnLeaveHover()
            {
                _isHovering = false;
                sprEnlarge.ResetSize();
            }
            public void OnClick()
            {
                if (IsGood)
                {
                    starSprRend.sprite = starClickedSpr;
                    starSprRend.color = hoverColor;
                }
                else
                {
                    starSprRend.color = wrongClickedColor;
                }
                sprEnlarge.ResetSize();
                sprEnlarge.enabled = false;
                HasClicked = true;
            }
            [Tooltip ("Check neighbors, if both clicked, connect linerender")]
            public void ConnectionCheck()
            {
                for (int i = 0; i < neighborStarsList.Count; i++)
                {
                    if (neighborStarsList[i].HasClicked && _lineRendList[i].gameObject.activeInHierarchy == false)
                    {
                        _lineRendList[i].gameObject.SetActive(true);
                        _lineRendList[i].SetPosition(0, transform.position);
                        _lineRendList[i].SetPosition(1, Vector3.Lerp(transform.position, neighborStarsList[i].transform.position,0.5f));
                        //Do the reverse position because its starting from the second star
                        _lineRendList[i].SetPosition(2, neighborStarsList[i].transform.position);
                    }
                }
            }
            public void ResetStar()
            {
                _isHovering = false;
                //hoverRotationQuaternion = Quaternion.Euler(hoverRotation);
            }
            private void StartFadeOut()
            {
                if (!HasClicked)
                {
                    isFading = true;
                    StartCoroutine(DoFadeOut());
                }
                
            }
            //public bool DEbug;
            IEnumerator DoFadeOut()
            {
                while (starSprRend.color.a > fadeoutAlpha)
                {
                    starSprRend.color = new Color(starSprRend.color.r, starSprRend.color.g, starSprRend.color.b, starSprRend.color.a - _fadeoutAmount);
                    //if(DEbug)print(starSprRend.color.a - _fadeoutAmount);
                    yield return new WaitForFixedUpdate();
                }
                starSprRend.color = new Color(starSprRend.color.r, starSprRend.color.g, starSprRend.color.b, fadeoutAlpha);
            }
            public List<StarBehavior_AS> GetNeighbors()
            {
                return neighborStarsList;
            }
            [Tooltip ("Rotate the star while ignoring the current state")]
            public void CelebrateRotate()
            {
                IsCelebrateRotation = true;
            }
            void EnableLineRenderer(bool _enable)
            {
                foreach (LineRenderer _line in _lineRendList)
                {
                    _line.enabled =_enable;
                }
            }
        }
    }
}