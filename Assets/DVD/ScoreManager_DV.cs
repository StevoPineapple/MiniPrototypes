using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RitualNight
{
    namespace PartyGames
    {
        public class ScoreManager_DV : MonoBehaviour
        {
            public int currentScore;
            public int targetScore;
            public int additionScore;
            // Start is called before the first frame update
            private void OnDrawGizmos()
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(transform.position+ transform.right * targetScore*0.5f, new Vector3(targetScore, 1, 1));
                Gizmos.color = Color.white;
                Gizmos.DrawCube(transform.position + transform.right * currentScore * 0.5f, new Vector3(currentScore, 1, 1));
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(transform.position + new Vector3(currentScore,0,0) + transform.right * additionScore*0.5f, new Vector3(additionScore, 1, 1));
            }
        }
    }
}