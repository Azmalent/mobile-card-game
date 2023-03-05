using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper.View
{
    public abstract class PlatformBase : MonoBehaviour
    {
        public float Angle => transform.localRotation.y;

        public abstract float Length { get; set; }
        public abstract Model.PlatformType Type { get; set; }
        public abstract float Height { get; }
        public abstract bool Visible { get; set; }
    }
}