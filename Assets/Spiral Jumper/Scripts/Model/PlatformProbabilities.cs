using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper.Model
{
    [Serializable]
    public class PlatformProbabilities
    {
        private static PlatformType[] types = {
            PlatformType.Simple85,
            PlatformType.Simple50,
            PlatformType.Simple35,
            PlatformType.Red50Left,
            PlatformType.Red50Right,
            PlatformType.Red50LeftRight
        };

        public PlatformType defaultType = PlatformType.Simple85;
        [Space]
        [Range(0, 100)] public float Simple85 = 0;
        [Range(0, 100)] public float Simple50 = 0;
        [Range(0, 100)] public float Simple35 = 1;
        [Range(0, 100)] public float Red50Left = 0;
        [Range(0, 100)] public float Red50Right = 0;
        [Range(0, 100)] public float Red50LeftRight = 0;


        public List<PropabilityRange> GetRanges()
        {
            float[] values = { Simple85, Simple50, Simple35, Red50Left, Red50Right, Red50LeftRight };
            float total = values.Sum();

            var ranges = new List<PropabilityRange>(values.Length);

            float begin = 0;
            for(int i = 0; i < values.Length; i++)
            {
                float p = values[i] / total;
                if (p > 0)
                {
                    float p1 = begin;
                    float p2 = begin + p;
                    begin = p2;
                    ranges.Add(new PropabilityRange { p1 = p1, p2 = p2, type = types[i] });
                }
            }
            if(ranges.Count == 0)
                ranges.Add(new PropabilityRange { p1 = 0, p2 = 1.0f, type = defaultType });

            return ranges;
        }

        public class PropabilityRange
        {
            public float p1;
            public float p2;
            public PlatformType type;
        }
    }
}