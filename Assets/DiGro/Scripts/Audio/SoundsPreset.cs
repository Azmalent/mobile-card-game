using System;
using System.Collections.Generic;

using UnityEngine;


namespace DiGro.Audio {

    [CreateAssetMenu(fileName = "sounds_preset.asset", menuName = "Custom/Sounds Preset", order = 51)]
    public class SoundsPreset : ScriptableObject {

        [Serializable]
        private class SoundItem {
            public SoundType type;
            public Sound sound;
        }

        [SerializeField] private List<SoundItem> m_soundsList = new List<SoundItem>();
        private Dictionary<SoundType, Sound> m_sounds = new Dictionary<SoundType, Sound>();


        public void Init() {
            foreach (var item in m_soundsList) {
                if (m_sounds.ContainsKey(item.type))
                    Debug.LogError("SoundsPreset: \"" + item.type.ToString() + "\" sound already added.");
                else
                    m_sounds.Add(item.type, item.sound);
            }

        }


        public Sound GetSound(SoundType type) {
            if (!m_sounds.ContainsKey(type))
                throw new Exception("SoundsPreset don't contain \"" + type.ToString() + "\" sound");
            return m_sounds[type];
        }

    }

}