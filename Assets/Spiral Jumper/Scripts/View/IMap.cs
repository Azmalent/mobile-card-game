using System;
using System.Collections.Generic;
using UnityEngine;

using Pool = DiGro.GameObjectsPool;

namespace SpiralJumper.View
{
    public interface IMap
    {
        View.PlatformBase CurrentPlatform { get; }
        View.PlatformBase NextPlatform { get; }

        void ToNextPlatform();
        void ToSavePlatform();
        void SetRedHeight(float height);

        View.PlatformBase GetPlatformFromCurrent(int offset);
    }
}