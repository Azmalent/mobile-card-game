using UnityEngine;
using System.Collections;

namespace DiGro.Cameras{

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class Scaler2D : MonoBehaviour {

        public float referencePixelPerUnit = 100f;
        public Vector2 referenceResolution = Vector2.zero;
        [Range(0, 1)]
        public float match = 0;

        private Camera m_camera = null;

        private Vector2 m_lastScreenSize = Vector2.zero;
        private float m_lastPPU = 0;
        private float m_lastMatch = 0;

        public bool i_resize = false;

        private void Awake() {
            m_camera = GetComponent<Camera>();
            m_lastScreenSize = new Vector2(Screen.width, Screen.height);
            m_lastPPU = referencePixelPerUnit;
            Resize();
        }

        private void Update() {
            bool mustResize = false;

            Vector2 resolution = new Vector2(Screen.width, Screen.height);
            if (resolution != m_lastScreenSize) {
                m_lastScreenSize = resolution;
                mustResize = true;
            }
            if (System.Math.Abs(m_lastPPU - referencePixelPerUnit) > 0.0001) {
                m_lastPPU = referencePixelPerUnit;
                mustResize = true;
            }
            if (m_lastMatch != match) {
                m_lastMatch = match;
                mustResize = true;
            }
            if (i_resize) {
                i_resize = false;
                mustResize = true;
            }
            if (mustResize)
                Resize();
        }

        private void Resize() {
            //float height = referenceResolution.y / m_lastScreenSize.x * m_lastScreenSize.y; // referencePixelPerUnit = 281.6666
            float byHeight = referenceResolution.y / m_lastScreenSize.y * m_lastScreenSize.y;
            float byWidht = referenceResolution.x / m_lastScreenSize.x * m_lastScreenSize.y;
            float delta = byHeight - byWidht;
            float height = byWidht + delta * match;

            float orthSize = height / referencePixelPerUnit / 2;
            m_camera.orthographicSize = orthSize;
        }

    }

}