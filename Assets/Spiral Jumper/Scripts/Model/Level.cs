using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper.Model
{
    [Serializable]
    public class Level
    {
        public int Index = -1;
        public List<Chank> chanks = new List<Chank>();
        public float beginHeight;
        public float endHeight;
        public float difficulty;

        public Chank FirstChank => chanks[0];

        public Chank LastChank => chanks[chanks.Count - 1];
    }
}