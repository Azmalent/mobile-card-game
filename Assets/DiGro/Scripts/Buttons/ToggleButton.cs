using UnityEngine;
using System.Collections;

namespace DiGro {

    public class ToggleButton : Button {
        [Header(nameof(ToggleButton))]

        [SerializeField] private GameObject m_onObject;
        [SerializeField] private GameObject m_offObject;

        [SerializeField] private bool m_startState = false;

        public bool ToggleActive {
            get { return m_startState; }
            set {
                m_startState = value;
                UpdateToggleState();
            }
        }


        protected override void Awake() {
            base.Awake();
            if (!m_onObject || !m_offObject)
                Debug.Log("Not all set in " + GetType());

            UpdateToggleState();
        }


        public void UpdateToggleState() {
            m_onObject.SetActive(ToggleActive);
            m_offObject.SetActive(!ToggleActive);
        }

    }

}