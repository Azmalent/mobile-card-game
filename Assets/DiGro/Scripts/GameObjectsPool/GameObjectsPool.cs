using UnityEngine;
using System;
using System.Collections.Generic;

namespace DiGro {

    public static class GameObjectsPool {

        private static Dictionary<string, Pool> m_pools = new Dictionary<string, Pool>();
        private static GameObject m_root = null;


        public static GameObject Pop(string poolName) {
            if (!m_pools.ContainsKey(poolName))
                throw new ArgumentException("Pool with name \"" + poolName + "\" not exist. Befor use pool you need create him.");

            Pool pool = m_pools[poolName];
            if (pool.empty)
                pool.Create();

            return pool.Pop();
        }

        public static void Push(string poolName, GameObject obj) {
            if (!m_pools.ContainsKey(poolName))
                throw new ArgumentException("Pool with name \"" + poolName + "\" not exist. Befor use pool you need create him.");

            m_pools[poolName].Push(obj);
        }

        public static void CreatePool(
                                        string poolName,
                                        GameObject prefab,
                                        IPoolListener poolListener,
                                        int startCount = 0,
                                        int maxCount = -1) {
            if (m_pools.ContainsKey(poolName))
                throw new ArgumentException("Pool with name \"" + poolName + "\" already exist.");

            if (!m_root) {
                GameObject founded = GameObject.FindGameObjectWithTag("GameObjectsPool");
                if (founded == null) {
                    m_root = new GameObject("GameObjectsPool");
                    m_root.tag = "GameObjectsPool";
                } else {
                    m_root = founded;
                }
            }
            GameObject poolObj = new GameObject(poolName);
            poolObj.transform.parent = m_root.transform;

            Pool pool = new Pool(poolObj, prefab, poolListener, startCount, maxCount);
            m_pools.Add(poolName, pool);
        }

        public static void SetMaxCount(string poolName, int value) {
            if (!m_pools.ContainsKey(poolName))
                throw new ArgumentException("Pool with name \"" + poolName + "\" not exist. Befor use pool you need create him.");

            m_pools[poolName].maxCount = value;
        }

        public static void Clear(string poolName) {
            if (!m_pools.ContainsKey(poolName))
                throw new ArgumentException("Pool with name \"" + poolName + "\" not exist. Befor use pool you need create him.");

            m_pools[poolName].Clear();
        }

        public static void Clear() {
            foreach (string poolName in m_pools.Keys)
                m_pools[poolName].Clear();
            m_pools.Clear();
        }

        public static bool HasPool(string poolName) {
            return m_pools.ContainsKey(poolName);
        }


        private class Pool {

            private Queue<GameObject> m_queue = new Queue<GameObject>();
            private GameObject m_prefab;
            private IPoolListener m_listener;
            private GameObject m_poolObject;
            private int m_startCount = 0;
            private int m_maxCount = -1;

            public bool empty { get { return m_queue.Count == 0; } }
            public GameObject poolObject { get { return m_poolObject; } }
            public int maxCount { get { return m_maxCount; } set { m_maxCount = value; } }


            public Pool(GameObject obj, GameObject prefab, IPoolListener listener, int startCount, int maxCount) {
                m_prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
                m_listener = listener ?? throw new ArgumentNullException(nameof(listener));
                m_poolObject = obj;
                m_startCount = startCount;
                m_maxCount = maxCount;

                if (m_startCount > 0) {
                    for (int i = 0; i < m_startCount && i < m_maxCount; i++)
                        Create();
                }
            }


            public GameObject Pop() {
                GameObject obj = m_queue.Dequeue();
                if (m_listener != null)
                    m_listener.OnPop(obj);
                return obj;
            }

            public void Push(GameObject obj) {
                if (m_queue.Count < m_maxCount || m_maxCount < 0) {
                    m_queue.Enqueue(obj);
                    obj.transform.parent = m_poolObject.transform;
                    if (m_listener != null)
                        m_listener.OnPush(obj);
                } else {
                    if (m_listener != null)
                        m_listener.OnPushFail(obj);
                }
            }

            public void Create() {
                GameObject obj = UnityEngine.Object.Instantiate(m_prefab);
                if (m_listener != null)
                    m_listener.OnCreate(obj);
                Push(obj);
            }

            public void Clear() {
                while (m_queue.Count > 0) {
                    GameObject obj = m_queue.Dequeue();
                    if (m_listener != null)
                        m_listener.OnClear(obj);
                }
            }

        }

    }

}