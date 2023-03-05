using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SpiralJumper.Audio;
using SpiralJumper.Amorphus;


namespace SpiralJumper.Controller
{
    [Serializable]
    public class JumpStat
    {
        public float lastJumpLength = 0;
        public float avJampLength = 0;
        [Space]
        public float lastJampTime = 0;
        public float avJampTime = 0;
        [Space]
        public float lastJampHeight = 0;
        public float avJampHeight = 0;
        public float maxJumpHeight = 0;
        [Space]
        public int jumpCount = 0;
        [Space]
        public int lastOnNextPlatform = 0;
        public float lastOnNextPlatformP = 0;
        public int avOnNextPlatform = 0;
        public float avOnNextPlatformP = 0;
        [Space]
        public int LastOnOtherPlatform = 0;
        public float LastOnOtherPlatformP = 0;
        public int avOnOtherPlatform = 0;
        public float avOnOtherPlatformP = 0;

        [Space]
        public bool reset = false;


        private float m_jumpLength = 0;
        private float m_totalJampLength = 0;

        private float m_jumpTime = 0;
        private float m_totalJampTime = 0;

        private float m_jumpHeight = 0;
        private float m_totalJampHeight = 0;

        private int m_onNextPlatform = 0;
        private int m_totalOnNextPlatform = 0;

        private int m_onOtherPlatform = 0;
        private int m_totalOnOtherPlatform = 0;

        private float m_startTime = 0;
        private float m_angle = 0;

        private Vector3 m_startPlayerPos;
        private Vector3 m_lastPlayerPos;


        //private SpiralJumper.DebugInfo m_debugInfo = null;


        public void Update(Vector3 playerPos, View.PlatformBase nextPlatform)
        {
            UpdateStat(playerPos, nextPlatform);
            if (reset)
            {
                reset = false;
                ResetStat();
            }
        }

        public void LateUpdate()
        {
            //SpiralJumper.get.PrintDebug(" jump: " + jumpCount.ToString());
        }

        public void UpdateStat(Vector3 playerPos, View.PlatformBase nextPlatform)
        {
            if (m_lastPlayerPos != playerPos)
            {
                m_jumpLength += (playerPos - m_lastPlayerPos).magnitude;
                m_jumpHeight = Mathf.Max(m_jumpHeight, playerPos.y - m_startPlayerPos.y);

                m_lastPlayerPos = playerPos;

                if (nextPlatform == null)
                    return;

                RaycastHit hit;
                if (Physics.Raycast(new Ray(playerPos, Vector3.down), out hit))
                {
                    Debug.DrawLine(playerPos, hit.point, Color.yellow);
                    var platformInfo = hit.collider.gameObject.GetComponent<View.PlatformEffector>();
                    if (platformInfo != null)
                    {
                        if (platformInfo.Platform == nextPlatform)
                            m_onNextPlatform++;
                        else
                            m_onOtherPlatform++;
                    }
                }
            }
        }

        public void StartStep(Vector3 playerPos, float angle)
        {
            m_startTime = Time.time;
            m_startPlayerPos = playerPos;

            m_jumpLength = 0;
            m_jumpTime = 0;
            m_jumpHeight = 0;

            m_onNextPlatform = 0;
            m_onOtherPlatform = 0;

            m_angle = angle;

            //if (m_debugInfo == null)
            //    m_debugInfo = SpiralJumper.get.CreateDebugInfo();
        }

        public void StopStep(Vector3 playerPos)
        {
            jumpCount++;

            m_jumpTime = Time.time - m_startTime;

            lastJumpLength = m_jumpLength;
            lastJampTime = m_jumpTime;
            lastJampHeight = m_jumpHeight;
            lastOnNextPlatform = m_onNextPlatform;
            LastOnOtherPlatform = m_onOtherPlatform;

            m_totalJampLength += m_jumpLength;
            m_totalJampTime += m_jumpTime;
            m_totalJampHeight += m_jumpHeight;
            m_totalOnNextPlatform += m_onNextPlatform;
            m_totalOnOtherPlatform += m_onOtherPlatform;

            avJampLength = m_totalJampLength / jumpCount;
            avJampTime = m_totalJampTime / jumpCount;
            avJampHeight = m_totalJampHeight / jumpCount;
            avOnNextPlatform = m_totalOnNextPlatform / jumpCount;
            avOnOtherPlatform = m_totalOnOtherPlatform / jumpCount;

            maxJumpHeight = Mathf.Max(maxJumpHeight, m_jumpHeight);

            float count = m_onNextPlatform + m_onOtherPlatform;
            lastOnNextPlatformP = m_onNextPlatform / count * 100;
            LastOnOtherPlatformP = m_onOtherPlatform / count * 100;

            float totalCount = avOnNextPlatform + avOnOtherPlatform;
            avOnNextPlatformP = avOnNextPlatform / totalCount * 100;
            avOnOtherPlatformP = avOnOtherPlatform / totalCount * 100;

            //if (m_debugInfo == null)
            //    m_debugInfo = SpiralJumper.get.CreateDebugInfo();

            //m_debugInfo.Value = "jump: " + jumpCount.ToString() + " height:" + Math.Round(m_jumpHeight, 2).ToString();
            SpiralJumper.get.Log("jumpStat\t" + jumpCount.ToString() + "\t" + m_jumpHeight.ToString() + "\t" + m_angle.ToString());
        }

        public void ResetStat()
        {
            jumpCount = 0;

            m_totalJampLength = 0;
            avJampLength = 0;

            m_totalJampTime = 0;
            avJampTime = 0;

            m_totalJampHeight = 0;
            avJampHeight = 0;
            maxJumpHeight = 0;

            m_totalOnNextPlatform = 0;
            avOnNextPlatformP = 0;

            m_totalOnOtherPlatform = 0;
            avOnOtherPlatformP = 0;

            m_angle = 0;

            //if (m_debugInfo != null)
            //{
            //    m_debugInfo.Remove();
            //    m_debugInfo = null;
            //}
        }
    }
}
