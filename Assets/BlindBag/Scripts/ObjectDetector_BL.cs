using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RitualNight
{
    namespace PartyGames
    {
        public class ObjectDetector_BL : MonoBehaviour
        {
            // Start is called before the first frame update
            private CircleCollider2D _selfCollider;
            public bool IsOnObject;
            public bool IsOnBag;
            [SerializeField] private ExpressionController_BL ExpressionController;
            [SerializeField] private PlayerController_BL PlayerController;

            public void OnStartGame()
            {
                _selfCollider.GetComponent<CircleCollider2D>();
            }
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision.CompareTag("Object_BL"))
                {
                    ExpressionController.Exclaim();
                    ExpressionController.EggCheckingBG();
                }
            }
            private void OnTriggerStay2D(Collider2D collision)
            {
                if (collision.CompareTag("Object_BL"))
                {
                    IsOnObject = true;
                }
                if (collision.CompareTag("BagArea_BL"))
                {
                    IsOnBag = true;
                }
            }
            private void OnTriggerExit2D(Collider2D collision)
            {
                if (collision.CompareTag("Object_BL"))
                {
                    ExpressionController.EggSearchingBG();
                    IsOnObject = false;
                }
                if (collision.CompareTag("BagArea_BL"))
                {
                    IsOnBag = false;
                }
            }
        }
    }
}
