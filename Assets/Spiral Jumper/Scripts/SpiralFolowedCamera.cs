using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralPlatformer
{
    public class SpiralFolowedCamera : CenteredCamera
    {
        [Header(nameof(SpiralFolowedCamera))]
        public GameObject m_centerRigid = null;
        public GameObject m_playerRigid = null;
        [Space]
        public float folowSpeed = 15;
        public Vector3 offset;

        public bool freezeAngle = false;
        
        private float m_lastCenterY;
        private Vector3 m_lastPlayerPosition;

        private Vector3 m_currentCenter;
        private Vector3 m_targetCenter;


        protected override void Awake() { }

        private void Start()
        {
            if (!m_centerRigid || !m_playerRigid)
                Debug.LogError("Not all set in " + GetType());

            updateMethod = UpdateMethod.None;

            m_lastCenterY = m_centerRigid.transform.position.y;
            m_targetCenter = m_centerRigid.transform.position;

            m_currentCenter = m_centerRigid.transform.position;
            m_Center = m_currentCenter + offset;

            UpdateAngle();
        }

        private void Update()
        {
            //UpdateAngle();
            //UpdateCenter();
            //UpdateCamera();
        }

        protected override void LateUpdate()
        {
            UpdateAngle();
            //UpdateCenter();
            UpdateCamera();

            base.LateUpdate();
        }

        protected override void FixedUpdate()
        {
            UpdateCenter();
            //UpdateAngle();
            UpdateCamera();

            base.FixedUpdate();
        }

        private void UpdateCenter ()
        {
            m_targetCenter = m_centerRigid.transform.position;
            if (m_currentCenter != m_targetCenter)
            {
                var pos = Vector3.Lerp(m_currentCenter, m_targetCenter, Time.fixedDeltaTime * folowSpeed);
                m_currentCenter = pos;
                m_Center = m_currentCenter + offset;
            }
        }

        private void UpdateAngle()
        {
            if (freezeAngle)
                return;

            var p13 = m_playerRigid.transform.position;
            var p12 = new Vector2(p13.x, p13.z);
            //var p23 = m_centerRigid.transform.position;
            //var p22 = new Vector2(p23.x, p23.z);

            AngleX = -Vector2.SignedAngle(Vector2.up, p12);
            //float targetAngle = -Vector2.SignedAngle(Vector2.up, p12);

            //AngleX = Mathf.LerpAngle(AngleX, targetAngle, Time.fixedDeltaTime * rotateSens);
        }

        //public void SetNextAnglePosition(float angle, Vector3 position) {
        //    AngleX = angle;

        //    m_targetCenter = position;
        //    if (m_currentCenter != m_targetCenter) {
        //        var pos = Vector3.Lerp(m_currentCenter, m_targetCenter, Time.deltaTime * folowSpeed);
        //        m_currentCenter = pos;
        //        m_Center = m_currentCenter + offset;
        //    }
        //    UpdateCamera();
        //}

    }
}