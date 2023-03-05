using System;
using System.Collections;

using UnityEngine;


namespace DiGro.ScreenSystem {

    public static class Extentions {
        public static Coroutine StartDeleyed(this MonoBehaviour monoBehaviour, Action action, float time) {
            return monoBehaviour.StartCoroutine(InvokeImpl(action, time));
        }

        private static IEnumerator InvokeImpl(Action action, float time) {
            yield return new WaitForSeconds(time);
            action();
        }
    }
}