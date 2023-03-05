using UnityEngine;
using UnityEngine.Audio;
using System.Collections;


namespace DiGro.Audio {

    [CreateAssetMenu(fileName = "sound.asset", menuName = "Custom/Sound", order = 51)]
    public class Sound : ScriptableObject {

        public enum OnAlreadyPlayAction { Break, Pass, Play }


        public AudioClip audioClip;
        public OnAlreadyPlayAction onAlreadyPlayAction = OnAlreadyPlayAction.Break;
        public bool loop = false;
        
        [Range(0, 1)]
        public float volume = 1;

        [Range(-3f, 3f)]
        public float pitch = 1;


    }

}