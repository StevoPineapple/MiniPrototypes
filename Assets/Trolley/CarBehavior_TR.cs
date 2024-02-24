using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class CarBehavior_TR : MonoBehaviour
        {
            public TrolleyTaskBehavior TRManager;
            public PlayerController_TR PlayerController;
            private Animator _anim;
            private void Awake()
            {
                _anim = GetComponent<Animator>();
            }
            public void CheckWin()
            {
                TRManager.WinCheck();
            }
            public void CheckSecondTrack()
            {
                if (_anim.GetBool("isFirstTrack") && !_anim.GetBool("isSecondTrack"))
                {
                    PlayerController.ReachSecondTrack();
                }
                else if (!_anim.GetBool("isFirstTrack") && _anim.GetBool("isSecondTrack"))
                {
                    PlayerController.EndSecondTrack();
                }
            }
            public void StartSecondTrack()
            {
                _anim.SetBool("isSecondTrack",false);
            }
        }
    }
}