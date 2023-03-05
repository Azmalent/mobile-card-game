using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpiralJumper.View
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private Slider m_slider;
        [SerializeField] private Image m_image;
        [SerializeField] private AnimationCurve m_curve;
        public Button button;


        private void Awake()
        {
            DiGro.Check.NotNull(m_slider);
            DiGro.Check.NotNull(m_image);
            DiGro.Check.NotNull(m_curve);
            DiGro.Check.NotNull(button);
        }

        public void Init(CharacterDescriptor desc)
        {
            name = "Character: " + desc.name;
            m_image.sprite = desc.sprite;
            int volume = PlayerPrefs.GetInt(desc.VolumeTag, 0);
            UpdateStorage(volume, 1000);
        }

        public void UpdateStorage(int volume, int maxVolume)
        {
            if (m_slider.maxValue != maxVolume)
                m_slider.maxValue = maxVolume;

            m_slider.value = volume;
        }

        public void ChangeValue(int value, int targetValue, int maxValue, float duration)
        {
            int target = Mathf.Clamp(targetValue, 0, maxValue);
            var changer = new ValueChanger(value, target, duration, m_curve);
            StartCoroutine(changer.Invoke((int v) => UpdateStorage(v, maxValue)));
        }


    }
}