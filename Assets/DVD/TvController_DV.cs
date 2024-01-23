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
            [SerializeField] private SpriteShapeController tvShape;
            [SerializeField] private PlayerController_DV playerController;
            [SerializeField] private EdgeCollider2D edgeCollider;
            [SerializeField] private ScoreManager_DV scoreManager;

            [SerializeField] private bool isGrabbing;
            [SerializeField] private int _currentCornerIndex;

            [SerializeField] private KnobBehavior_DV[] knobArr;
            [SerializeField] private GameObject[] cornerArr;

            public LogoBehavior_DV CurrentLogo;

            [SerializeField] private LineRenderer[] lineArr;

            public int BounceSide;

            // Start is called before the first frame update
            public void StartGame()
            {
                foreach (KnobBehavior_DV _knob in knobArr)
                {
                    _knob.SetInitPos();
                }
            }
            public void ResetGame()
            {
                foreach (KnobBehavior_DV _knob in knobArr)
                {
                    _knob.ReturnToInitPos();
                }
            }

            // Update is called once per frame 
            void Update()//Put a trigger on every side -> determine which side is hit. on dvd logo
            {
                for (int i = 0; i < tvShape.spline.GetPointCount(); i++)
                {
                    cornerArr[i].transform.position = edgeCollider.points[i];
                    lineArr[i].SetPosition(0, edgeCollider.points[i]);
                    lineArr[i].SetPosition(1, edgeCollider.points[i + 1 > 3 ? 0 : i + 1]);

                    if (i != BounceSide) 
                    {
                        lineArr[i].startColor = Color.blue;
                        lineArr[i].endColor = Color.blue;
                    }
                }
                if (isGrabbing)
                {
                    tvShape.spline.SetPosition(_currentCornerIndex, playerController.gameObject.transform.position - tvShape.transform.position);
                    tvShape.BakeCollider();
                    tvShape.BakeMesh();

                    knobArr[_currentCornerIndex].transform.position = playerController.gameObject.transform.position - tvShape.transform.position;
                    
                    tvShape.spline.SetPosition(_currentCornerIndex, knobArr[_currentCornerIndex].ClampPosition());//weird maybe youhua

                }
                //tvShape.spline.SetPosition(4, tvShape.spline.GetPosition(0)+new Vector3());

                RaycastHit2D[] rayD = Physics2D.LinecastAll(edgeCollider.points[0], edgeCollider.points[1]);
                //lineArr[0].SetPosition(0, edgeCollider.points[0]);
                //lineArr[0].SetPosition(1, edgeCollider.points[1]);
                RaycastHit2D[] rayL = Physics2D.LinecastAll(edgeCollider.points[1], edgeCollider.points[2]);
                RaycastHit2D[] rayU = Physics2D.LinecastAll(edgeCollider.points[2], edgeCollider.points[3]);
                RaycastHit2D[] rayR = Physics2D.LinecastAll(edgeCollider.points[3], edgeCollider.points[0]);


                foreach (RaycastHit2D ray in rayD) //This WILL NOT WORK IF THERE IS A HUUUGE LAG SPIKE will do for now
                {
                    if (ray.collider.CompareTag("Detector_DV"))
                    {
                        print("Down");
                        lineArr[0].startColor = Color.yellow;
                        lineArr[0].endColor = Color.yellow;
                        BounceSide = 0;
                        break;
                    }
                }
                foreach (RaycastHit2D ray in rayL)
                {
                    if (ray.collider.CompareTag("Detector_DV"))
                    {
                        print("Left");
                        lineArr[1].startColor = Color.yellow;
                        lineArr[1].endColor = Color.yellow;
                        BounceSide = 1;
                        break;
                    }
                }
                foreach (RaycastHit2D ray in rayU)
                {
                    if (ray.collider.CompareTag("Detector_DV"))
                    {
                        print("Up");
                        lineArr[2].startColor = Color.yellow;
                        lineArr[2].endColor = Color.yellow;
                        BounceSide = 2;
                        break;
                    }
                }
                foreach (RaycastHit2D ray in rayR)
                {
                    if (ray.collider.CompareTag("Detector_DV"))
                    {
                        print("Right");
                        lineArr[3].startColor = Color.yellow;
                        lineArr[3].endColor = Color.yellow;
                        BounceSide = 3;
                        break;
                    }
                }


            }
            public void KnobGrabbed(int _cornerIndex)
            {
                isGrabbing = true;
                _currentCornerIndex = _cornerIndex;
            }
            public void ExitGrab()
            {
                isGrabbing = false;
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