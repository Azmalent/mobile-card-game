using UnityEngine;
using System;
using System.Collections.Generic;

namespace DiGro.Localization {

    public class Manager : SingletonMono<Manager> {

        public static bool Initialized { get; private set; }
        public static event Action OnLocalizationChange;

        private static Dictionary<Tag, string> m_dict = new Dictionary<Tag, string>();
        

        [SerializeField] private SystemLanguage i_applicationLanguage = SystemLanguage.Unknown;
        [SerializeField] private bool i_useThis = false;

        private SystemLanguage tmp_lastLanguage;


        private void Awake() {
            tmp_lastLanguage = i_applicationLanguage;
        }

        private void Update() {
            if(tmp_lastLanguage != i_applicationLanguage && i_useThis) {
                tmp_lastLanguage = i_applicationLanguage;
                Init();
            }
        }


        public static void Init() {
            m_dict.Clear();

            var language = Application.systemLanguage;
            if (get.i_useThis)
                language = get.i_applicationLanguage;

            LocalizationData eng = null;
            LocalizationData current = null;
            var assets = Resources.LoadAll<LocalizationData>("Localization");
            foreach(var asset in assets) {
                if (asset.language == SystemLanguage.English)
                    eng = asset;
                if (asset.language == language)
                    current = asset;
            }
            var localization = current != null ? current : eng;
            foreach(var localizedString in localization.strings) 
                m_dict.Add(localizedString.tag, localizedString.value);

            Initialized = true;
            OnLocalizationChange?.Invoke();
        }

        public static string GetString(Tag tag) {
            if (!Initialized)
                Init();

            if (m_dict.ContainsKey(tag))
                return m_dict[tag];

            return "NOT_LOCALIZED";
        }
    }

}