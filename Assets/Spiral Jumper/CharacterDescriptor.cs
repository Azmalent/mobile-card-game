using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpiralJumper
{
    [Serializable]
    public class CharacterDescriptor
    {
        public string name;
        public GameObject prefab;
        public Sprite sprite;

        public string VolumeTag => name + ".volume";
    }
}
