using UnityEngine;
using System.Collections.Generic;

namespace DiGro.ScreenSystem {

    [CreateAssetMenu(fileName = "animation_list.asset", menuName = "Custom/Screen System/Animation List", order = 51)]
    public class AnimationList : ScriptableObject {
        [SerializeField] private List<DelayedAnimation> m_list = new List<DelayedAnimation>();

        public int Count { get { return m_list.Count; } }

        public DelayedAnimation this[int index] {
            get { return m_list[index]; }
            set { m_list[index] = value; }
        }

    }
}