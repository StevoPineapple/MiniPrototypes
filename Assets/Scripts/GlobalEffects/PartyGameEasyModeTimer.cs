using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RitualNight
{
    namespace PartyGames
    {
        public class PartyGameEasyModeTimer : MonoBehaviour
        {
            [SerializeField] private float easyModeStartTime;
            private Coroutine easyModeTimer;
            public bool IsEasyMode;
            public UnityEvent onEasyModeStartEvt;
            // Start is called before the first frame update
            public void OnReset()
            {
                if (easyModeTimer != null)
                {
                    StopCoroutine(easyModeTimer);
                    easyModeTimer = null;
                }
                 IsEasyMode = false;
            }
            public void StartTimer()
            {
                if (easyModeTimer == null)
                {
                    easyModeTimer = StartCoroutine(DoEasyModeTimer());
                }
            }

            private IEnumerator DoEasyModeTimer()
            {
                yield return new WaitForSeconds(easyModeStartTime);
                IsEasyMode = true;
                onEasyModeStartEvt.Invoke();
            }
            private void OnDisable()
            {
                OnReset();
            }
        }
    }
}
