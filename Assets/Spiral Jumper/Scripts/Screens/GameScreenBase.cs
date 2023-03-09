using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DiGro.Input;

namespace SpiralJumper.Screens
{
    public abstract class GameScreenBase : MonoBehaviour
    {
        public abstract event Action ContinueEvent;
        public abstract event Action RestartEvent;
        public abstract event Action StartEvent;

        public virtual bool HasSecondPlayer { get; set; }

        public abstract void OnGameOver();
        public abstract void OnGameScoreChenged(int value);
        public abstract void OnLevelChanged(int value);
        public abstract void OnLongFallBegin();
        public abstract void OnLongFallEnd();

        public virtual void OnGameContinue() { }
        public virtual void OnGameStart() { }
        public virtual void OnRedZoneDistanceChanged(float value) { }
        public virtual void OnRedZoneHeightChanged(float value) { }
        public virtual void OnCurrentLevelHeightChanged(float value) { }
    }
}