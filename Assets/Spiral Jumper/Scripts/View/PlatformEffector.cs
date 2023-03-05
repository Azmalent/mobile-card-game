using System;
using System.Collections.Generic;
using UnityEngine;

/// TODO: 
/// Определение эффекта по позиции игрока.


namespace SpiralJumper.View
{
    public class PlatformEffector : MonoBehaviour
    {
        [SerializeField] private List<Sector> m_sectors = new List<Sector>();
        
        public PlatformBase Platform { get; set; }
        public int SectorsCount => m_sectors.Count;

        public bool IsHilighted(int sector) => m_sectors[sector].IsHilight;
        public void SetHilight(int sector, bool value) => m_sectors[sector].IsHilight = value;
        public Model.PlatformEffect GetEffect(int sector) => m_sectors[sector].effect;

        private float m_endAngle = 0;


        private void Awake()
        {
            if (SectorsCount == 0)
                Debug.LogError("Not all set in " + GetType());

            float beginAngle = 0;
            for (int i = 0; i < SectorsCount; i++)
            {
                m_endAngle = beginAngle + m_sectors[i].angle;
                beginAngle = m_endAngle;
            }
        }

        public float GetAngle(Vector3 position)
        {
            var vecToPoint = position - transform.position;
            var vecToForward = Platform.transform.rotation * Vector3.forward;

            float angle = Vector3.SignedAngle(vecToForward, vecToPoint, Vector3.up);
            if (angle < 0)
                angle += 360;

            return angle;
        }

        public int GetSector(float angle)
        {
            float beginAngle = 0;
            for (int i = 0; i < SectorsCount; i++)
            {
                var sector = m_sectors[i];
                float endAngle = beginAngle + sector.angle;
                if (angle >= beginAngle && angle <= endAngle)
                    return i;

                beginAngle = endAngle;
            }
            return -1;
        }

        public int GetExtremeSector(float angle)
        {
            if (SectorsCount == 1)
                return 0;

            float deltaToBegin = angle >= 180 ? 360 - angle : angle;
            float deltaToEnd = Mathf.Abs(m_endAngle - angle);

            return deltaToBegin <= deltaToEnd ? 0 : SectorsCount - 1;
        }

        public int GetSectorByContactPoint(Vector3 point)
        {
            float angle = GetAngle(point);
            int sector = GetSector(angle);
            if (sector < 0)
                sector = GetExtremeSector(angle);

            return sector;
        }


        [Serializable]
        public class Sector
        {
            public float angle = 0;
            public Model.PlatformEffect effect = Model.PlatformEffect.None;
            public GameObject hilightObj = null;

            public bool IsHilight
            {
                get => m_isHilight;
                set
                {
                    m_isHilight = value;
                    if (hilightObj != null)
                        hilightObj.SetActive(m_isHilight);
                }
            }

            private bool m_isHilight = false;
        }
    }
}