using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace DiGro.Localization {

    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour {

        [SerializeField] private Tag m_tag;
        public bool useLocalization = true;

        public Text text { get; private set; }
        
        public Tag LocalizationTag {
            get { return m_tag; }
            set {
                m_tag = value;
                LoadLocalizedString();
            }
        }


        private void Awake() {
            text = GetComponent<Text>();
            Manager.OnLocalizationChange += OnLocalizationChange;
            LoadLocalizedString();
        }

        private void OnDestroy() {
            Manager.OnLocalizationChange -= OnLocalizationChange;
        }


        private void LoadLocalizedString() {
            if (useLocalization)
                text.text = Manager.GetString(m_tag);
        }

        private void OnLocalizationChange() {
            LoadLocalizedString();
        }
        
    }

}