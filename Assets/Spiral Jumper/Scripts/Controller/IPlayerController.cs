using System;
using UnityEngine;

namespace SpiralJumper.Controller
{
    public interface IPlayerController
    {
        event Action<View.PlatformEffector, int> PlatformEnterListener;

        Vector3 Position { get; }
        Vector3 Scale { get; }

        bool IsJump { get; }
        bool IsFall { get; }

        void Init(View.IMap mapView);
    }
}