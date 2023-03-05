using System;
using System.Collections.Generic;
using UnityEngine;

using UIButton = UnityEngine.UI.Button;


namespace DiGro
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class ToggleUButton : MonoBehaviour
    {
        [SerializeField] private GameObject m_onRoot;
        [SerializeField] private GameObject m_offRoot;

        [SerializeField] private bool m_startState = false;

        public UIButton.ButtonClickedEvent OnClick => m_onClick;

        public bool ToggleActive
        {
            get { return m_startState; }
            set
            {
                m_startState = value;
                UpdateToggleState();
            }
        }

        private UIButton m_button = null;
        private UIButton.ButtonClickedEvent m_onClick = new UIButton.ButtonClickedEvent();


        protected void Awake()
        {
            if (!m_onRoot || !m_offRoot)
                Debug.Log("Not all set in " + GetType());

            m_button = GetComponent<UIButton>();
            m_button.onClick = m_onClick;
            UpdateToggleState();
        }


        public void UpdateToggleState()
        {
            m_onRoot.SetActive(ToggleActive);
            m_offRoot.SetActive(!ToggleActive);
        }
    }
}