using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper.View
{
    public class Platform : PlatformBase
    {
        [SerializeField] private GameObject m_visibleObj = null;
        [SerializeField] private GameObject m_invisibleObj = null;

        [SerializeField] private bool m_visible = true;

        public override float Length { get; set; }
        public override Model.PlatformType Type { get; set; }

        public override float Height => transform.position.y;

        public override bool Visible
        {
            get => m_visible;
            set
            {
                m_visible = value;
                m_visibleObj.SetActive(m_visible);
                m_invisibleObj.SetActive(!m_visible);
            }
        }


        private void Awake()
        {
            if (!m_visibleObj || !m_invisibleObj)
                Debug.LogError("Not all set in " + GetType());

            var effectors = GetComponentsInChildren<PlatformEffector>();
            foreach (var effector in effectors)
                effector.Platform = this;

            Visible = m_visible;
        }

    }
}