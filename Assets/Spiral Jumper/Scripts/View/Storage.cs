using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpiralJumper.View
{
    public class Storage : MonoBehaviour
    {
        [SerializeField] private Text m_valueText;
        [SerializeField] private AnimationCurve m_curve;

        public int Value
        {
            get => m_value;
            set
            {
                m_value = value;
                m_valueText.text = m_value.ToString();
            }
        }

        private int m_value;
        
        private void Awake()
        {
            DiGro.Check.NotNull(m_valueText);
            DiGro.Check.NotNull(m_curve);
        }

        private void Update()
        {

        }

        public void ChangeValue(int targetValue, float duration)
        {
            var changer = new ValueChanger(Value, targetValue, duration, m_curve);
            StartCoroutine(changer.Invoke((int v) => Value = v));
        }
        
    }
}