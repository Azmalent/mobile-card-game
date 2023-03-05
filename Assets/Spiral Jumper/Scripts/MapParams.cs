using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper
{
    [CreateAssetMenu(fileName = "MapParams.asset", menuName = "Custom/SpiralJumper/MapParams", order = 51)]
    public class MapParams : ScriptableObject
    {
        public Model.MapParams data;
    }
}
