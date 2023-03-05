using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper.Model
{
    [Serializable]
    public class Platform
    {
        public PlatformType type;
        public float height;
        public float angle;
    }
}