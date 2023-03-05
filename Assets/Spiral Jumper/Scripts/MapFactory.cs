using System;
using System.Collections.Generic;
using UnityEngine;

using Check = DiGro.Check;
using Pool = DiGro.GameObjectsPool;


namespace SpiralJumper
{
    public class MapFactory : DiGro.SingletonMono<MapFactory>
    {
        private const string RedZonePoolName = "RZ";
        private const string PillarPoolName = "P";

        [SerializeField] private GameObject m_pillarPrefab = null;
        [SerializeField] private GameObject m_redZonePrefab = null;
        public PlatformsInfo platformsInfo = null;

        private Dictionary<Model.PlatformType, Item> m_dict = new Dictionary<Model.PlatformType, Item>();


        public PlatformsInfo.PlatformInfo PlatformInfo(Model.PlatformType platformType)
        {
            CheckContains(platformType);
            return m_dict[platformType].info;
        }

        public GameObject Platform(Model.PlatformType platformType)
        {
            string poolName = "P" + ((int)platformType).ToString();
            return Pool.Pop(poolName);
        }

        public void PushPlatform(GameObject obj, Model.PlatformType platformType)
        {
            string poolName = "P" + ((int)platformType).ToString();
            Pool.Push(poolName, obj);
        }

        public GameObject Pillar() => Pool.Pop(PillarPoolName);
        public void PushPillar(GameObject obj) => Pool.Push(PillarPoolName, obj);

        public GameObject RedZone() => Pool.Pop(RedZonePoolName);
        public void PushRedZone(GameObject obj) => Pool.Push(RedZonePoolName, obj);


        private void Awake()
        {
            Check.NotNull(m_pillarPrefab);
            Check.NotNull(m_redZonePrefab);
            Check.NotNull(platformsInfo);

            foreach (var platformInfo in platformsInfo.list)
            {
                Check.CheckComponent<View.PlatformBase>(platformInfo.prefab);
                CheckDuplicate(platformInfo.type);

                string poolName = "P" + ((int)platformInfo.type).ToString();

                m_dict.Add(platformInfo.type, new Item() { 
                    poolName = poolName,
                    info = platformInfo
                });

                if (!Pool.HasPool(poolName))
                    Pool.CreatePool(poolName, platformInfo.prefab, new DiGro.SimplePoolListener(), 30, 100);
            }

            if (!Pool.HasPool(PillarPoolName))
                Pool.CreatePool(PillarPoolName, m_pillarPrefab, new DiGro.SimplePoolListener(), 3, 100);

            if (!Pool.HasPool(RedZonePoolName))
                Pool.CreatePool(RedZonePoolName, m_redZonePrefab, new DiGro.SimplePoolListener(), 1, 100);
        }

        
        private void CheckDuplicate(Model.PlatformType platformType)
        {
            if (m_dict.ContainsKey(platformType))
                throw new Exception("Duplicates in Platform Factory.");
        }

        private void CheckContains(Model.PlatformType platformType)
        {
            if (!m_dict.ContainsKey(platformType))
                throw new Exception("Platform Factory not contains PlatformType: " + platformType.ToString() + ".");
        }


        private class Item
        {
            public string poolName;
            public PlatformsInfo.PlatformInfo info;
        }
    }
}