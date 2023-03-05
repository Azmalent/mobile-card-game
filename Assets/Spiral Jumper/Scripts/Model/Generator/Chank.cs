using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper.Model
{
    [Serializable]
    public class Chank
    {
        public List<Platform> platforms = new List<Platform>();
        public float beginHeight;
        public float endHeight;
        public float difficulty;

        public bool HasPlatforms => platforms.Count != 0;

        public Platform FirstPlatform => platforms[0];

        public Platform LastPlatform => platforms[platforms.Count - 1];
    }
}