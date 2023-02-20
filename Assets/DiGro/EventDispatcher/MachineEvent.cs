using System;
using UnityEngine.Events;
using Unity.VisualScripting;

namespace DiGro {

    [Inspectable]
    public class MachineAction {

        [Inspectable]
        public ScriptMachine machine;

        [Inspectable]
        public string method;

        private int m_hash = 0;


        public int GetHash() {
            if (m_hash == 0)
                m_hash = ($"{machine.GetHashCode()}" + $"{method.GetHashCode()}").GetHashCode();

            return m_hash;
        }

        public override int GetHashCode() {
            return GetHash();
        }

    }
}