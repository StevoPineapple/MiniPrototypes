using RitualNight.PartyGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class CandleParticle_CRD : MonoBehaviour
        {
            [SerializeField] private float speed;
            [SerializeField] private float minDistance;
            private Vector2 direction;
            public CardsTaskBehavior CRDManager;
            public void FlyToCandle(GameObject _candle)
            {
                StartCoroutine(DoFlyToCandle(_candle));
            }
            IEnumerator DoFlyToCandle(GameObject _candle)
            {
                while (Vector2.Distance(transform.position, _candle.transform.position) > minDistance)
                {
                    if (Vector3.Angle(direction, _candle.transform.position - transform.position) >= 90)
                    {
                        print("break");
                        break;
                    }
                    direction = _candle.transform.position - transform.position;
                    transform.position += (Vector3)direction.normalized * speed * Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                CRDManager.LightCandle();
                Destroy(gameObject);
            }
        }
    }
}