using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirror;
using UnityEngine;

namespace DiGro {

    public class SingletonMirror<T> : NetworkBehaviour where T : MonoBehaviour {

        private static T m_instance;

        public static T get {
            get {
                if (m_instance == null) {
                    m_instance = FindObjectOfType<T>();
                    if (m_instance == null)
                        Debug.LogError("Can't find" + typeof(T) + "!");
                }
                return m_instance;
            }
        }

        public static bool HasInstance {
            get { return m_instance != null; }
        }

    }

}
