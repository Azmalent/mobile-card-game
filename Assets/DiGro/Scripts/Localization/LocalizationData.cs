using System;
using System.Collections.Generic;

using UnityEngine;


namespace DiGro.Localization {

    [CreateAssetMenu(fileName = "localization.asset", menuName = "Custom/Localization", order = 51)]
    public class LocalizationData : ScriptableObject {

        public SystemLanguage language = SystemLanguage.Unknown;
        public List<LocalizedString> strings = new List<LocalizedString>();


        [Serializable]
        public class LocalizedString {
            public Tag tag;
            public string value;
        }

    }

}