using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper
{
    [CreateAssetMenu(fileName = "DifficultParams.asset", menuName = "Custom/SpiralJumper/DifficultParams", order = 51)]
    public class DifficultParams : ScriptableObject
    {
        public DifficultLoop firstBigLoop = new DifficultLoop() { min = 1, max = 5, length = 10 };
        [Space]
        public DifficultLoop bigLoop = new DifficultLoop() { min = 1, max = 5, length = 10 };
        [Space]
        public DifficultLoop smallLoop = new DifficultLoop() { min = 0, max = 0.5f, length = 1 };

        [Serializable]
        public class DifficultLoop
        {
            public AnimationCurve curve;

            [Header("Minimum difficult")]
            public float min;

            [Header("Maximum difficult")]
            public float max;

            [Header("Loop length in levels")]
            public int length; 
        }
    }
}
