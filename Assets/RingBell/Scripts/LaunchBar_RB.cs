using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class LaunchBar_RB : MonoBehaviour
        {
            [Header("Components")]
            [SerializeField] private GameObject currTriangleObj;
            [SerializeField] private ParallaxManager_RB parallaxManager;
            [SerializeField] private GameObject[] triangleObjArr;
            private Triangle_RB[] _triangleArr = new Triangle_RB[5];

            [Header("States")]
            [SerializeField] private int totalHitTimes;
            private bool _isHammerDown;

            [Header("Bar")]
            private float barOffSet;
            [SerializeField] private GameObject barFillObject;

            [SerializeField] private float rotateSpeed;

            [SerializeField] private float maxRotateSpeed;
            [SerializeField] private float rotateSpeedInit;
            private bool _isRotateLeft;

            [SerializeField] private float maxAngleOneSide;
            [SerializeField] private float hitDelay;
            private float _maxAngleFull;
            [SerializeField] private float rotateSpeedIncreaseMulti;

            private float rotateAngle;
            [Header("Sliding")]
            
            private bool _isTriSliding;
            [SerializeField] private float slideSpeed;
            [SerializeField] private float slideDACCLerpInit;
            [SerializeField] private float[] slideDACCLerpArr = new float[5];
            [SerializeField] private float[] slideColorAdditionArr = new float[5]; //0 is blue
            [SerializeField] private float slideDACCLerpDecrease;
            [SerializeField] private float minSlideSpeedThreshold;


            private float[] pastTriangleAngleArr = new float[5];
            private int[] pastTriSignArr = new int[5];



            [Header ("Color")]
            [SerializeField] private float barBlueAngle;
            [SerializeField] private float barGreenAngle;
            [SerializeField] private float barYellowAngle;
            [SerializeField] private float barOrangeAngle;
            [SerializeField] private float barRedAngle;
            
            [SerializeField] private Color barBlueCol;
            [SerializeField] private Color barGreenCol;
            [SerializeField] private Color barYellowCol;
            [SerializeField] private Color barOrangeCol;
            [SerializeField] private Color barRedCol;

            [Header("Hammer")]
            [SerializeField] private GameObject flashScreen;

            [SerializeField] private SpriteRenderer hammerSprRend;
            [SerializeField] private Sprite hammerIdle;
            [SerializeField] private Sprite[] hammerRaiseArr;
            [SerializeField] private Sprite[] hammerSmashArr;
            private int _maxHammerCount = 5;
            private int hammerCount;
            [SerializeField] private float totalErrorPoints;
            [SerializeField] private float errorPointsAdj;

            [Header("Ghost")]
            private float[] _ghostAngleArr = new float[5];
            [SerializeField] private GameObject[] ghostTriangleArr = new GameObject[5];
            [SerializeField] private int ghostDelay;

            [Header("Debug")]
            public bool DebugMode;
            public bool DebugLaunch;
            public bool DebugBlendMode;

            private void Awake()
            {
                _maxAngleFull = maxAngleOneSide * 2;
                for (int i = 0; i < 5; i++)
                {
                    _triangleArr[i] = triangleObjArr[i].GetComponent<Triangle_RB>();
                }
            }
            public void StartGame()
            {
                ResetGame();
                flashScreen.SetActive(false);
                triangleObjArr[0].SetActive(true);
                StartCoroutine(DoSetGhostAngle());

            }

            // Update is called once per frame
            public float debugEAz;
            public float debugOffset;
            void Update()
            {
                if (DebugLaunch)
                {
                    OnMousePress();
                }
                if (Input.GetKeyDown(KeyCode.R) && DebugMode)
                {
                    StartGame();
                }

                if (Input.GetKeyDown(KeyCode.S) && DebugMode)
                {
                    currTriangleObj.GetComponent<Triangle_RB>().Pass(1);
                }
                if (!_isTriSliding&&!_isHammerDown)
                {
                    RotateTriangle(rotateSpeed);
                    CheckPassTriangle(ref rotateSpeed);
                }
                //currTriangle.transform.rotation = Quaternion.Euler(currTriangle.transform.rotation.eulerAngles.x, currTriangle.transform.rotation.eulerAngles.y, rotateAngle);

                if (Mathf.Abs(barFillObject.transform.rotation.eulerAngles.z - barOffSet) > 0.001f)
                {
                    debugEAz = barFillObject.transform.rotation.eulerAngles.z;
                    debugOffset = barOffSet;
                    float _adjAngle = barFillObject.transform.rotation.eulerAngles.z>180? barFillObject.transform.rotation.eulerAngles.z-360: barFillObject.transform.rotation.eulerAngles.z;



                    barFillObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(_adjAngle, barOffSet, 0.8f));
                }

                if (_isTriSliding)
                {
                    DebugBlendTriangle(Mathf.Abs(barOffSet - rotateAngle));
                }

                if (DebugMode&&DebugBlendMode&&currTriangleObj!=null)
                {
                    BlendTriangle(Mathf.Abs(barOffSet - rotateAngle));
                }
            }

            IEnumerator DoSetGhostAngle()
            {
                while (true)
                {
                    for (int i = 5-1; i >= 0; i--)
                    {
                        if (i == 0)
                        {
                            _ghostAngleArr[i] = rotateAngle;
                        }
                        else
                        {
                            _ghostAngleArr[i] = _ghostAngleArr[i - 1];
                        }
                        ghostTriangleArr[i].transform.rotation = Quaternion.Euler(0, 0, _ghostAngleArr[i]);
                    }
                    yield return new WaitForEndOfFrame();
                }
            }

            private void RotateTriangle(float _rotateSpeed)
            {
                Vector3 _triEulerAngles = currTriangleObj.transform.rotation.eulerAngles;
                if (_isRotateLeft)
                {   
                    rotateAngle = Mathf.Clamp(rotateAngle - (_rotateSpeed * Time.deltaTime), -maxAngleOneSide, maxAngleOneSide);
                    currTriangleObj.transform.rotation = Quaternion.Euler(_triEulerAngles.x, _triEulerAngles.y, rotateAngle);
                    if (rotateAngle == -maxAngleOneSide)
                    {
                        SetAllTriangleSign();
                        _isRotateLeft = false;
                    }
                }
                else
                {
                    rotateAngle = Mathf.Clamp(rotateAngle + (_rotateSpeed * Time.deltaTime), -maxAngleOneSide, maxAngleOneSide);
                    currTriangleObj.transform.rotation = Quaternion.Euler(_triEulerAngles.x, _triEulerAngles.y, rotateAngle);
                    if (rotateAngle == maxAngleOneSide)
                    {
                        SetAllTriangleSign();
                        _isRotateLeft = true;
                    }
                }
            }
            private void CheckPassTriangle(ref float _rotateSpeed)
            {
                for (int i = 0; i < hammerCount; i++)
                {
                    int _currSign = (int)Mathf.Sign(rotateAngle - pastTriangleAngleArr[i]);
                    if (_currSign!=pastTriSignArr[i])
                    {
                        _triangleArr[i].Pass(_currSign);
                        _rotateSpeed = Mathf.Clamp(_rotateSpeed + rotateSpeedIncreaseMulti,0,maxRotateSpeed);
                        pastTriSignArr[i] = _currSign;
                    }
                }
            }

            private void SetAllTriangleSign()
            {
                for (int i = 0; i < hammerCount; i++)
                {
                    pastTriSignArr[i] = (int)Mathf.Sign(rotateAngle - pastTriangleAngleArr[i]);
                }
            }

            public void OnMousePress()
            {
                if (_isHammerDown||_isTriSliding)
                {
                    return;
                }
                if (hammerCount < _maxHammerCount)
                {
                    ReleaseTriangle();
                }
            }

            void ReleaseTriangle()
            {
                StartCoroutine(DoReleaseTriangle());
            }
            IEnumerator DoReleaseTriangle()
            {
                _isTriSliding = true;
                slideSpeed = rotateSpeed;
                while (slideSpeed>=minSlideSpeedThreshold)
                {
                    RotateTriangle(slideSpeed);
                   // CheckPassTriangle(ref slideSpeed);
                    slideSpeed = Mathf.Lerp(slideSpeed, 0, slideDACCLerpArr[hammerCount]); 
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(0.4f);
                

                //slideDACCLerp -= slideDACCLerpDecrease;
                slideSpeed = 0;
                
                RaiseHammer();
            }
            void RaiseHammer()
            {

                float _adjustedAngle = Mathf.Abs(barOffSet - rotateAngle);
                totalErrorPoints += _adjustedAngle;
                BlendTriangle(_adjustedAngle);
                _isTriSliding = false;

                barOffSet = -Mathf.Sign(rotateAngle) * Random.Range(0, maxAngleOneSide);
                pastTriangleAngleArr[hammerCount] = rotateAngle;
                
                hammerSprRend.sprite = hammerRaiseArr[hammerCount];
                ghostTriangleArr[hammerCount].SetActive(true);

                hammerCount++;
                if (hammerCount == _maxHammerCount)
                {
                    currTriangleObj = null;
                    SmashHammer();
                    return;
                }
                triangleObjArr[hammerCount].SetActive(true);
                currTriangleObj = triangleObjArr[hammerCount];
                currTriangleObj.transform.rotation = Quaternion.Euler(0, 0, rotateAngle);
                if (_isRotateLeft)
                {
                    rotateAngle -= 0.0001f;
                    currTriangleObj.transform.Rotate(0, 0, -0.0001f);
                    pastTriSignArr[hammerCount-1] = -1;
                }
                else
                {
                    rotateAngle += 0.0001f;
                    currTriangleObj.transform.Rotate(0, 0, 0.0001f);
                    pastTriSignArr[hammerCount-1] = 1;
                }
                
            }
            void DebugBlendTriangle(float _angle)
            {
                if (_angle < barBlueAngle)
                {
                    _triangleArr[hammerCount].BlendColor(barBlueCol);
                }
                else if (_angle < barGreenAngle)
                {
                    _triangleArr[hammerCount].BlendColor(barGreenCol);
                }
                else if (_angle < barYellowAngle)
                {
                    _triangleArr[hammerCount].BlendColor(barYellowCol);
                }
                else if (_angle < barOrangeAngle)
                {
                    _triangleArr[hammerCount].BlendColor(barOrangeCol);
                }
                else
                {
                    _triangleArr[hammerCount].BlendColor(barRedCol);
                }
            }
            void BlendTriangle(float _angle)
            {
                if (_angle < barBlueAngle)
                {
                    rotateSpeed += slideColorAdditionArr[0];
                    _triangleArr[hammerCount].BlendColor(barBlueCol);
                    totalErrorPoints -= 1.5f;//
                }
                else if (_angle < barGreenAngle)
                {
                    rotateSpeed += slideColorAdditionArr[1];
                    _triangleArr[hammerCount].BlendColor(barGreenCol);
                }
                else if (_angle < barYellowAngle)
                {
                    rotateSpeed += slideColorAdditionArr[2];
                    _triangleArr[hammerCount].BlendColor(barYellowCol);
                }
                else if (_angle < barOrangeAngle)
                {
                    rotateSpeed += slideColorAdditionArr[3];
                    _triangleArr[hammerCount].BlendColor(barOrangeCol);
                }
                else
                {
                    rotateSpeed += slideColorAdditionArr[4];
                    _triangleArr[hammerCount].BlendColor(barRedCol);
                }
            }
            void SmashHammer()
            {
                _isHammerDown = true;
                StartCoroutine(DoHammerAnimation());
            }
            public float debug4;
            void Launch()
            {
                if (DebugMode)
                {
                    parallaxManager.Launch(debug4);
                    return;
                }
                parallaxManager.Launch(totalErrorPoints);
            }
            public void ResetGame()
            {
                rotateSpeed = rotateSpeedInit;
                rotateAngle = -maxAngleOneSide;

                barOffSet = 0;
                barFillObject.transform.rotation = Quaternion.identity;

                hammerCount = 0;
                hammerSprRend.sprite = hammerIdle;
                _isHammerDown = false;

                //slideDACCLerp = slideDACCLerpArr;
                _isTriSliding = false;

                totalErrorPoints = 0;

                foreach (Triangle_RB _tri in _triangleArr)
                {
                    _tri.BlendColor(Color.white);
                    _tri.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    _tri.gameObject.SetActive(false);
                }
                for(int i=0;i<5; i++)
                {
                    _ghostAngleArr[i] = 0;
                    ghostTriangleArr[i].SetActive(false);

                    pastTriangleAngleArr[i] = 0;
                    pastTriSignArr[i] = 0;
                }

                currTriangleObj = triangleObjArr[0];
                StopAllCoroutines();
            }
            IEnumerator DoHammerAnimation()
            {
                yield return new WaitForSeconds(0.2f);

                hammerSprRend.sprite = hammerSmashArr[0];
                yield return new WaitForSeconds(0.025f);

                hammerSprRend.sprite = hammerSmashArr[1];
                yield return new WaitForSeconds(0.025f);

                hammerSprRend.sprite = hammerSmashArr[2];
                flashScreen.SetActive(true);
                yield return new WaitForSeconds(hitDelay);
                flashScreen.SetActive(false);
                Launch();
            }
        }
    }
}