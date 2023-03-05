using System;
using UnityEngine;

namespace SpiralRunner {

    public class VirtualStick : MonoBehaviour {

        public event Action<float> StickMoveEvent;

        public float deadArea = 0.02f;
        public float deadDelta = 0.01f;
        public float timeToDeadArea = 0.3f;

        private float m_lastPos;
        private float m_basePos;
        private float m_direction;
        private float m_moveDelta;

        private bool m_hasTouch = false;
        private bool m_inDeadArea;
        private float m_timeToChange;

        public float Direction => m_direction;
        public float MoveDelta => m_moveDelta;


        public void Start() {
            
        }

        public bool HandleInput() {
            var pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            bool isTouch = Input.GetMouseButton(0);
            if (isTouch && !m_hasTouch) m_OnBeginTouch();
            if (!isTouch && m_hasTouch) m_OnEndTouch();

            m_moveDelta = pos.x - m_lastPos;

            if (!m_hasTouch)
                return false;

            if (m_inDeadArea) {
                m_inDeadArea = Mathf.Abs(m_moveDelta) < deadArea;
                if (!m_inDeadArea)
                    m_moveDelta -= deadArea;
            }
            if (!m_inDeadArea) {
                //bool inDeadDelta = Mathf.Abs(m_moveDelta) < deadDelta;
                //if (!m_inDeadArea && inDeadDelta)
                //    m_timeToChange -= Time.deltaTime;
                //else
                //    m_timeToChange = timeToDeadArea;

                //if (!m_inDeadArea && m_timeToChange <= 0) {
                //    m_inDeadArea = true;
                //    m_timeToChange = timeToDeadArea;
                //    m_lastPos = pos.x;

                //    //Debug.Log($"VirtualStick: Dead Area {m_moveDelta}");
                //    return false;
                //}

                //if (inDeadDelta)
                //    return false;

                m_lastPos = pos.x;

                float currentDirection = Mathf.Sign(m_moveDelta);
                if (m_moveDelta != 0 && currentDirection != 0 && currentDirection != m_direction) {
                    m_direction = currentDirection;

                    //Debug.Log($"VirtualStick: Change Direction {m_moveDelta}, {(m_direction > 0 ? "Right" : "Left")}");
                }
                else {
                    //Debug.Log($"VirtualStick: Move {(m_direction > 0 ? "Right" : "Left")}");
                }
                return true;
                //StickMoveEvent?.Invoke(m_direction);
            }
            return false;
        }

        private void m_OnBeginTouch() {
            var pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            m_basePos = pos.x;
            m_lastPos = m_basePos;
            m_direction = 0;
            m_timeToChange = timeToDeadArea;
            m_inDeadArea = true;

            m_hasTouch = true;
        }

        private void m_OnEndTouch() {
            m_hasTouch = false;
        }

    }

}
