using System;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine;

namespace DiGro {

    public class MachineAction {

        public GameObject target;

        public string method;

        private int m_hash = 0;

        public MachineAction(GameObject target, string method) {
            this.target = target;
            this.method = method;
        }

        public int GetHash() {
            if (m_hash == 0)
                m_hash = ($"{target.GetHashCode()}" + $"{method.GetHashCode()}").GetHashCode();

            return m_hash;
        }

        public override int GetHashCode() {
            return GetHash();
        }

    }
}