using UnityEngine;
using System.Collections;

namespace DiGro {

    public interface IPoolListener {

        void OnPop(GameObject obj);
        void OnPush(GameObject obj);
        void OnPushFail(GameObject obj);
        void OnCreate(GameObject obj);
        void OnClear(GameObject obj);

    }

}