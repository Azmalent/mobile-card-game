using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper
{
    [CreateAssetMenu(fileName = "PlatformsInfo.asset", menuName = "Custom/SpiralJumper/Platforms Info", order = 51)]
    public class PlatformsInfo : ScriptableObject
    {
        public List<PlatformInfo> list = new List<PlatformInfo>();


        [Serializable]
        public class PlatformInfo
        {
            public string poolName;
            public GameObject prefab;
            public Model.PlatformType type;
            public float length;
            public float difficulty;
            public bool canGenerate = false;
        }
    }
}