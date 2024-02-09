using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace RitualNight
{
    namespace PartyGames
    {
        public class TvController_DV : MonoBehaviour
        {
            // Inconsistency: for loop through all sides sometimes uses hard code number 4 and not side count.
            // 
            //
            [Header ("Components")]
            [SerializeField] private PlayerController_DV playerController;
            [SerializeField] private SpriteShapeController tvShape;
            [SerializeField] private SpriteShapeController tvFillShape;
            [SerializeField] private SpriteShapeController tvSpawnArea;
            [SerializeField] private SpriteShapeController tvBufferWall;
            [SerializeField] private EdgeCollider2D edgeCollider;

            [Header ("Values")]
            public LogoBehavior_DV CurrentLogo;
            [SerializeField] private bool isGrabbing;
            
            [Header ("Sides")]
            //    2
            //  2---3
            // 1|   |3
            //  1---0
            //    0
            public int BounceSide; 
            public int GrabAltSide; //The other side related to corner
            [SerializeField] private LineRenderer[] lineArr;
            [SerializeField] private EdgeCollider2D[] sideDetectorArr;
            

            [Header("Bounce")]
            public int BounceCount;            
            [SerializeField] private int maxBounceCount;

            [Header ("Corners")]
            [SerializeField] private int _currentCornerIndex;
            public Transform[] initShapePosArr;
            [SerializeField] private KnobBehavior_DV[] knobArr;
            [SerializeField] private GameObject[] cornerArr;

            [Header ("Hang")]
            [SerializeField] private SpriteShapeController tvEyeShape;
            [SerializeField] private Sprite tvEyeNormal;
            [SerializeField] private Sprite tvEyeRight;
            [SerializeField] private Sprite tvEyeLeft;
            [SerializeField] private Sprite tvEyeHit;
            [SerializeField] private int tvEyeLightTime;
            [SerializeField] private int tvEyeLightTimeMax;

            [SerializeField] private SpriteShapeController tvBrandShape;

            [Header("UI")]
            [SerializeField] private GameObject barMeshObj;
            [SerializeField] private GameObject spdIconMeshObj;
            [SerializeField] private GameObject bounceCountMeshObj;
            [SerializeField] private MeshFilter[] barFillSegArr;
            [SerializeField] private Texture[] bounceCountTexArr;
            [SerializeField] private Texture[] spdIconArr;
            [SerializeField] private Texture emptyTex;


            private MeshRenderer _spdIconMeshRend;
            private MeshRenderer _bounceCountMeshRend;
            private MeshFilter _barMeshFilter;
            private MeshFilter _spdIconMeshFilter;
            private MeshFilter _bounceCountMeshFilter;
        

            [Header("BounceShift")]
            [SerializeField] private float shiftDistance;
            private float[] currShiftDistanceArr = new float[4];
            [SerializeField] private float returnLerp;
            private Vector2 _initPos;

            [Header("Color")]
            [SerializeField] private Color midSlowCol;
            [SerializeField] private Color slowCol;

            [Header("Side Flash")]
            [SerializeField] private int flashCycle;
            [SerializeField] private float flashCycleTime;

            private Color _cornerColor;
            private Color _lineColor = Color.yellow;
            private bool _isLineAltColor;
            [SerializeField] int _sideFlashTickMax;
            int _sideFlashTick;
            public bool IsReleaseFlashing;
            float[] _sideBlendAmountArr = new float[4];

            [Header ("Win")]
            [SerializeField] private int winFlashCycle;
            [SerializeField] private float winFlashCycleTime;
            public float WinFlashTotalTime; //used in create logo

            [Header("Debug")]
            public GameObject d;
            public int Debug1;
            public int Debug2;

            public int debugv0;
            public int debugv1;
            public int debugv2;
            public int debugv3;

            private void Awake()
            {
                _spdIconMeshRend = spdIconMeshObj.GetComponent<MeshRenderer>();
                _bounceCountMeshRend = bounceCountMeshObj.GetComponent<MeshRenderer>();

                _barMeshFilter = barMeshObj.GetComponent<MeshFilter>();
                _spdIconMeshFilter = spdIconMeshObj.GetComponent<MeshFilter>();
                _bounceCountMeshFilter = bounceCountMeshObj.GetComponent<MeshFilter>();
            }

            public void StartGame()
            {
                WinFlashTotalTime = winFlashCycle * winFlashCycleTime;
                _initPos = transform.position; //for bounce shift
                foreach (KnobBehavior_DV _knob in knobArr)
                {
                    _knob.SetInitPos();
                }
                ResetBounceCount();
                ResetCorners();
                BounceSide = -1;
                GrabAltSide = -1;

                for (int i = 0; i < tvShape.spline.GetPointCount(); i++)
                {
                    tvShape.spline.SetPosition(i, initShapePosArr[i].transform.position - transform.position);
                    cornerArr[i].transform.position = edgeCollider.points[i]+_initPos;
                    lineArr[i].SetPosition(0, edgeCollider.points[i]);
                    lineArr[i].SetPosition(1, edgeCollider.points[i + 1 > 3 ? 0 : i + 1]);

                    sideDetectorArr[i].SetPoints(new List<Vector2> { edgeCollider.points[i], edgeCollider.points[i + 1 > 3 ? 0 : i + 1]});

                    tvFillShape.spline.SetPosition(i, tvShape.spline.GetPosition(i));

                    lineArr[i].startColor = Color.blue;
                    lineArr[i].endColor = Color.blue;
                }

                tvEyeShape.spline.SetPosition(0, edgeCollider.points[2] + _initPos);
                tvEyeShape.spline.SetPosition(1, edgeCollider.points[3] + _initPos);

                tvBrandShape.spline.SetPosition(0, edgeCollider.points[0] + _initPos);
                tvBrandShape.spline.SetPosition(1, edgeCollider.points[1] + _initPos);

            }
            public void ResetGame()
            {
                StopAllCoroutines();
                foreach (KnobBehavior_DV _knob in knobArr)
                {
                    _knob.ReturnToInitPos();
                }
                ResetBounceCount();
                ResetCorners();
                BounceSide = -1;
                for (int i = 0; i < tvShape.spline.GetPointCount(); i++)
                {
                    lineArr[i].startColor = Color.blue;
                    lineArr[i].endColor = Color.blue;
                }
            }
            void LateUpdate()//Put a trigger on every side -> determine which side is hit. on dvd logo
            {
                // Change BG Color
                if (playerController.IsMidSlow)
                {
                    tvFillShape.spriteShapeRenderer.color = midSlowCol;
                }
                else if (playerController.IsSlow)
                {
                    tvFillShape.spriteShapeRenderer.color = slowCol;
                }
                else
                {
                    tvFillShape.spriteShapeRenderer.color = Color.white;
                }

                //ShiftBounce
                for (int i = 0; i < 4; i++)
                {
                    if (currShiftDistanceArr[i] > 0)
                    {
                        currShiftDistanceArr[i] = Mathf.Lerp(currShiftDistanceArr[i], -0.1f, returnLerp);
                    }
                    else
                    {
                        currShiftDistanceArr[i] = 0;
                    }
                }
                Vector2 _shiftPos = new Vector2(-currShiftDistanceArr[1] + currShiftDistanceArr[3], -currShiftDistanceArr[0] + currShiftDistanceArr[2]);
                transform.position = _initPos + _shiftPos;

                //Eyes
                HandleEyes();

                //Set stuff
                Vector2 _selfWorldPos = transform.position;
                if (isGrabbing)
                {
                    //border position
                    tvShape.spline.SetPosition(_currentCornerIndex, knobArr[_currentCornerIndex].ClampOtherPosition(playerController.gameObject.transform.position - tvShape.transform.position));
                    tvShape.BakeCollider();
                    tvShape.BakeMesh();

                    //Knob position
                    knobArr[_currentCornerIndex].transform.position = knobArr[_currentCornerIndex].ClampOtherPosition(playerController.gameObject.transform.position - tvShape.transform.position);

                    //Eye position and Tv Brand
                    HandleAccessory();
                }

                //Set UI Mesh
                Vector3[] screenVertexPos = _barMeshFilter.mesh.vertices;

                for (int i = 0; i < tvShape.spline.GetPointCount(); i++)
                {
                    //set corner sprite to edgeCollider
                    cornerArr[i].transform.position = edgeCollider.points[i] + _selfWorldPos;

                    //Set Line Renderer
                    lineArr[i].SetPosition(0, edgeCollider.points[i]);
                    lineArr[i].SetPosition(1, edgeCollider.points[(i+1)%4]);

                    //Set Side Colliders
                    sideDetectorArr[i].SetPoints(new List<Vector2> { edgeCollider.points[i], edgeCollider.points[i + 1 > 3 ? 0 : i + 1] });

                    //Set UI Mesh
                    if (i == 0)
                    {
                        screenVertexPos[i] = tvShape.spline.GetPosition(1) + transform.position;
                    }
                    else if (i == 1)
                    {
                        screenVertexPos[i] = tvShape.spline.GetPosition(0) + transform.position;
                    }
                    else if (i == 2)
                    {
                        screenVertexPos[i] = tvShape.spline.GetPosition(2) + transform.position;
                    }
                    else if (i == 3)
                    {
                        screenVertexPos[i] = tvShape.spline.GetPosition(3) + transform.position;
                    }

                    //Set BG noise fill
                    tvFillShape.spline.SetPosition(i, tvShape.spline.GetPosition(i));
                    tvSpawnArea.spline.SetPosition(i, tvShape.spline.GetPosition(i));
                    tvBufferWall.spline.SetPosition(i, tvShape.spline.GetPosition(i));
                }

                foreach (MeshFilter _meshFilter in barFillSegArr)
                {
                    _meshFilter.mesh.vertices = screenVertexPos;
                }
                _barMeshFilter.mesh.vertices = screenVertexPos;
                _spdIconMeshFilter.mesh.vertices = screenVertexPos;
                _bounceCountMeshFilter.mesh.vertices = screenVertexPos;
            }

            public void AlignMeshToTv(MeshFilter _meshFilter, Vector3[] tvPosArr)
            {
                _meshFilter.mesh.vertices = tvPosArr;
            }
            
            public void ForceReleaseFlash(int _side)
            {
                StartCoroutine(DoForceReleaseFlash(_side));
                IsReleaseFlashing = true;
            }
            IEnumerator DoForceReleaseFlash(int _side)
            {
                playerController.CanGrab = false;
                CurrentLogo.StopMovement();
                int _bounceKnob = _side;
                int _bounceKnob2 = (_side+1)% 4;
                DisableAllKnobs();
                knobArr[_bounceKnob].IsStopped = true;
                knobArr[_bounceKnob2].IsStopped = true;

                TurnAllSidesBlue();
                ResetBounceCount();
                ResetCorners();

                int _currCycle = 0;
                bool _blackCycle = true;
                Color _cycleColor = Color.black;
                while (_currCycle < flashCycle)
                {
                    CurrentLogo.StopMovement(); //redundant to make sure it doesn't move
                    if (_blackCycle)
                    {
                        _cycleColor = Color.black;
                    }
                    else
                    {
                        _cycleColor = Color.red;
                    }
                    _blackCycle = !_blackCycle;
                    lineArr[_side].startColor = _cycleColor;
                    lineArr[_side].endColor = _cycleColor;
                    knobArr[_bounceKnob].BGSprite.color = _cycleColor;
                    knobArr[_bounceKnob2].BGSprite.color = _cycleColor;
                    _currCycle++;
                    yield return new WaitForSeconds(flashCycleTime);
                }
                
                knobArr[_bounceKnob].IsStopped = false;
                knobArr[_bounceKnob2].IsStopped = false;
                CurrentLogo.ResumeMovement();
                ResetBounceCount();
                IsReleaseFlashing = false;
            }

           

            public void ResetLineCornerColor() //called when logo hits wall
            {
                foreach (KnobBehavior_DV _knob in knobArr)
                {
                    _knob.ResetAltColor();
                }
                _lineColor = Color.yellow;
            }
            public void BounceShift(int _dir)
            {
                currShiftDistanceArr[_dir] = shiftDistance;
            }

            public void LightUpEyes()
            {
                tvEyeLightTime = tvEyeLightTimeMax;
                tvEyeShape.spriteShape.angleRanges[0].sprites[0] = tvEyeHit;
            }
            private void HandleEyes()
            {
                //TvEyes
                if (tvEyeLightTime > 0)
                {
                    tvEyeLightTime--;
                }
                else
                {
                    tvEyeShape.spriteShape.angleRanges[0].sprites[0] = tvEyeNormal;
                }
                if (isGrabbing)
                //TvEyes Left Right
                if (_currentCornerIndex == 1 || _currentCornerIndex == 2)
                {
                    tvEyeShape.spriteShape.angleRanges[0].sprites[0] = tvEyeLeft;
                }
                else if (_currentCornerIndex == 0 || _currentCornerIndex == 3)
                {
                    tvEyeShape.spriteShape.angleRanges[0].sprites[0] = tvEyeRight;
                }

            }
            private void HandleAccessory() 
            {
                tvEyeShape.spline.SetPosition(0, edgeCollider.points[2]);
                tvEyeShape.spline.SetPosition(1, edgeCollider.points[3]);

                tvBrandShape.spline.SetPosition(0, edgeCollider.points[0]);
                tvBrandShape.spline.SetPosition(1, edgeCollider.points[1]);
            }
            public void ChangeSpeedIcon(int _spd)
            {
                _spdIconMeshRend.material.mainTexture = spdIconArr[_spd];
            }
            public void ShowBounceSide(int _side)
            {
                if (IsReleaseFlashing)
                {
                    return;
                }
                BounceSide = _side;
                for (int i = 0; i < 4; i++)
                {
                    if (i != BounceSide)
                    {
                        lineArr[i].startColor = Color.blue;
                        lineArr[i].endColor = Color.blue;
                    }
                }
                lineArr[BounceSide].startColor = Color.yellow;
                lineArr[BounceSide].endColor = Color.yellow;
            }
            private void TurnAllSidesBlue()
            {
                for (int i = 0; i < 4; i++)
                {
                    lineArr[i].startColor = Color.blue;
                    lineArr[i].endColor = Color.blue;
                }
            }
            private void NearSideFlash()
            {
                if (!isGrabbing || IsReleaseFlashing)
                {
                    return;
                }
                for (int i = 0; i < 4; i++)
                {
                    //Make side blue
                    if (i == BounceSide || i == GrabAltSide && _sideBlendAmountArr[i] < 1)
                    {
                        if (_isLineAltColor)
                        {
                            lineArr[i].startColor = new Color(1, _sideBlendAmountArr[i], 0, 1);
                            lineArr[i].endColor = new Color(1, _sideBlendAmountArr[i], 0, 1);
                        }
                        else
                        {
                            lineArr[i].startColor = Color.yellow;
                            lineArr[i].endColor = Color.yellow;
                        }
                        _sideFlashTick--;
                    }
                    else
                    {
                        lineArr[i].startColor = Color.blue;
                        lineArr[i].endColor = Color.blue;
                    }
                }
                    
                if (_sideFlashTick <= 0)
                {
                    _isLineAltColor = !_isLineAltColor;
                    _sideFlashTick = _sideFlashTickMax;
                    return;
                }
            }
            private void FixedUpdate()
            {
                NearSideFlash();
            }
            public void BlendSideColor(int _sideIndex ,float _amount)
            {

                _sideBlendAmountArr[_sideIndex] = _amount;
                int _knob2 = (_sideIndex + 1)% 4;
                knobArr[_sideIndex].BlendColor(_amount);
                knobArr[_knob2].BlendColor(_amount);
            }
            public void IncreaseBounceCount()
            {
                BounceCount = Mathf.Clamp(BounceCount + 1,0,maxBounceCount);
                _bounceCountMeshRend.material.mainTexture = bounceCountTexArr[BounceCount - 1];
            }
            public void ResetBounceCount()
            {
                BounceCount = 0;
                playerController.DVManager.ResetPotentialScore();
                _bounceCountMeshRend.material.mainTexture = emptyTex;
            }
            public void ReleaseOppositeCorners()
            {
                StartCoroutine(DoShowCorner(cornerArr[(BounceSide + 2) % 4]));

                StartCoroutine(DoShowCorner(cornerArr[(BounceSide + 3) % 4])); 
            }
            public void ResetCorners()
            {
                foreach (GameObject _cornerObj in cornerArr)
                {
                    _cornerObj.SetActive(false);
                }
            }
            public void ResetWinCorners(GameObject _cornObj)
            {
                foreach (GameObject _cornerObj in cornerArr)
                {
                    if (_cornerObj == _cornObj)
                    {
                        _cornerObj.GetComponent<SpriteRenderer>().color = Color.white;
                        StartCoroutine(ShowWinCorner(_cornObj));
                    }
                    else
                    {
                        _cornerObj.SetActive(false);
                    }
                }
            }
            IEnumerator DoShowCorner(GameObject _showCorner)
            {
                if(!_showCorner.activeInHierarchy)
                {
                    _showCorner.GetComponent<SpriteRenderer>().color = Color.white;
                }
                yield return new WaitForSeconds(0.15f);
                _showCorner.GetComponent<SpriteRenderer>().color = _cornerColor;
                _showCorner.SetActive(true);
            }
            IEnumerator ShowWinCorner(GameObject _winCorner) 
            {
                int _currCycle = 0;
                bool _falseCycle = false;
                while (_currCycle < winFlashCycle)
                {
                    _winCorner.SetActive(_falseCycle);
                    _falseCycle = !_falseCycle;
                    _currCycle++;
                    yield return new WaitForSeconds(winFlashCycleTime);
                }
                _winCorner.SetActive(false);
            }
            public void CloseCorners()
            {
                foreach (GameObject _cornerObj in cornerArr)
                {
                    _cornerObj.SetActive(true);
                }
                cornerArr[BounceSide].SetActive(false);
                cornerArr[BounceSide + 1 > 3 ? 0 : BounceSide + 1].SetActive(false);
            }

            public void KnobGrabbed(int _cornerIndex)
            {
                print(_cornerIndex);
                isGrabbing = true;
                _currentCornerIndex = _cornerIndex;
                if (BounceSide == _cornerIndex)
                {
                    GrabAltSide = _cornerIndex - 1 < 0 ? 3 : _cornerIndex - 1;
                }
                else
                {
                    GrabAltSide = _cornerIndex;
                }
                lineArr[GrabAltSide].startColor = Color.yellow;
                lineArr[GrabAltSide].endColor = Color.yellow;
                print(GrabAltSide);
            }
            public void KnobReleased()
            {
                ShowBounceSide(BounceSide);
                isGrabbing = false;
                GrabAltSide = -1;
            }
            public void DisableAllKnobs()
            {
                foreach (KnobBehavior_DV _knob in knobArr)
                {
                    _knob.TurnRed();
                }
            }
            public void NewCornerColor(Color _col)
            {
                foreach (GameObject _obj in cornerArr)
                {
                    _obj.GetComponent<SpriteRenderer>().color = _col;
                }
                _cornerColor = _col;
            }
            public void UpdateCornerColor()
            {
                int b2 = BounceSide + 1 > 3 ? 0 : BounceSide + 1;
                for (int i = 0; i < 4; i++)
                {
                    if (i == BounceSide || i == b2)
                    {
                        knobArr[i].TurnYellow();
                    }
                    else
                    {
                        knobArr[i].TurnRed();
                    }
                }
            }
            public int[] CurrentCorner()
            {
                return new int[2] {BounceSide, BounceSide+1>3 ? 0 : BounceSide+1 };
            }
            public Vector3 GetTvCenter()
            {
                return tvShape.transform.position;
            }
            private void OnDrawGizmos()
            {
                for (int i = 0; i < tvShape.spline.GetPointCount(); i++)
                {
                    switch (i)
                    {
                        case (0): { Gizmos.color = Color.white;break; }
                        case (1): { Gizmos.color = Color.red; break; }
                        case (2): { Gizmos.color = Color.yellow; break; }
                        case (3): { Gizmos.color = Color.green; break; }
                        case (4): { Gizmos.color = Color.blue; break; }
                    }
                    Gizmos.DrawSphere(edgeCollider.points[i], 0.1f);
                    //Gizmos.DrawSphere(tvShape.spline.GetPosition(i) + tvShape.transform.position, 0.1f);
                }
            }


        }
    }
}