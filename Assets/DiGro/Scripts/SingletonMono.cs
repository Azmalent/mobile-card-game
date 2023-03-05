using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiGro
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static T mInstance;

        public static T get
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = FindObjectOfType<T>();
                    if (mInstance == null)
                        Debug.LogError("Can't find" + typeof(T) + "!");
                }
                return mInstance;
            }
        }

        public static bool HasInstance
        {
            get { return mInstance != null; }
        }

    }
}