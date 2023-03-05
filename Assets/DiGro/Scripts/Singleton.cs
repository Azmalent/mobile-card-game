using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DiGro
{
    public class Singleton<T> where T : class, new()
    {

        private static T m_instance = null;

        public static T get
        {
            get
            {
                if (m_instance == null)
                    m_instance = new T();

                return m_instance;
            }
        }
    }
}