using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DiGro {
    public class CounterButton : Button {
        [Header(nameof(CounterButton))]

        [SerializeField] private Text m_valueText = null;


        public int Value {
            get => m_value;
            set {
                m_value = value;
                m_valueText.text = m_value.ToString();
            }
        }

        private int m_value = 0;


        protected override void Awake() {
            base.Awake();
            if (!m_valueText)
                Debug.LogError("Not all set in " + GetType());
        }


    }

}