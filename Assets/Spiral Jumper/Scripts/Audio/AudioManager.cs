using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SpiralJumper.Audio {

    public class AudioManager : DiGro.SingletonMono<AudioManager> {

        public static bool audioEnabled = true;

        [SerializeField] private bool blocking = false;
        [SerializeField] private SoundsPreset m_preset = null;
        private Dictionary<SoundType, SoundWrapper> m_sounds = new Dictionary<SoundType, SoundWrapper>();

        [Header("Test")]
        public SoundType i_sound;
        public bool i_play = false;
        

        private void Awake() {
            if (!m_preset)
                Debug.LogError("Not all set in " + GetType());
            m_preset.Init();
        }

        private void Update() {
            if (i_play) {
                i_play = false;
                StartSound(i_sound);
            }
        }


        public static void StartSound(SoundType type) {
            if (!audioEnabled || get.blocking)
                return;

            if (!get.m_sounds.ContainsKey(type))
                get.m_sounds.Add(type, new SoundWrapper(get.m_preset.GetSound(type)));

            var wrapper = get.m_sounds[type];
            if (wrapper.gameObject == null) {
                wrapper.gameObject = new GameObject("Sound: " + type.ToString());
                wrapper.gameObject.transform.parent = get.gameObject.transform;
            }
            if (wrapper.source != null && wrapper.source.isPlaying) {
                if (wrapper.sound.onAlreadyPlayAction == Sound.OnAlreadyPlayAction.Pass)
                    return;
            }
            AudioSource source = null;
            if (wrapper.sound.onAlreadyPlayAction == Sound.OnAlreadyPlayAction.Play) {
                source = wrapper.gameObject.AddComponent<AudioSource>();
                Destroy(source, wrapper.sound.audioClip.length);
            } else {
                if (wrapper.source == null)
                    wrapper.source = wrapper.gameObject.AddComponent<AudioSource>();
                source = wrapper.source;
            }
            source.clip = wrapper.sound.audioClip;
            source.volume = wrapper.sound.volume;
            source.pitch = wrapper.sound.pitch;
            source.loop = wrapper.sound.loop;
            source.Play();
        }

        public static void StopSounds() {
            foreach(var pair in get.m_sounds) {
                if(pair.Value.source != null)
                    pair.Value.source.Stop();
            }
        }


        private class SoundWrapper {
            public Sound sound = null;
            public GameObject gameObject = null;
            public AudioSource source = null;

            public SoundWrapper(Sound sound) { this.sound = sound; }
        }

    }

}
