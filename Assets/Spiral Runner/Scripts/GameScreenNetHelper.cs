using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirror;
using UnityEngine;

using SR = SpiralRunner;

namespace SpiralRunner {

    public class GameScreenNetHelper : DiGro.SingletonMirror<GameScreenNetHelper> {

        public static event Action ContinueEvent;

        private int m_waitingСontinue = 0;

        [Command]
        public void CmdContinue() {
            Debug.Log("CmdContinue");

            m_waitingСontinue++;

            if (m_waitingСontinue == 2) {
                m_waitingСontinue = 0;

                TargetContinue(NetworkManager.get.firstClient);
                TargetContinue(NetworkManager.get.secondClient);
            }
        }

        [TargetRpc]
        public void TargetContinue(NetworkConnection target) {
            Debug.Log("TargetContinue");
            ContinueEvent?.Invoke();
        }

    }

}