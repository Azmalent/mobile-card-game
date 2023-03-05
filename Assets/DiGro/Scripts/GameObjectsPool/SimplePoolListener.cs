using UnityEngine;
using System.Collections;

namespace DiGro {

    public class SimplePoolListener : IPoolListener {

        public void OnCreate(GameObject obj) {
            obj.SetActive(false);
        }

        public void OnPop(GameObject obj) {
            obj.SetActive(true);
        }

        public void OnPush(GameObject obj) {
            obj.SetActive(false);
        }

        public void OnClear(GameObject obj) {
            DeleteObj(obj);
        }

        public void OnPushFail(GameObject obj) {
            DeleteObj(obj);
        }

        private void DeleteObj(GameObject obj) {
#if (UNITY_EDITOR)
            Object.DestroyImmediate(obj);
#else
		Object.Destroy(obj);
#endif
        }


    }

}