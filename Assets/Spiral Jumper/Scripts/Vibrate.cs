using UnityEngine;
using System.Collections;


public static class Vibrate {

    private const int FINE = 50;
    private const int FAIL = 100;
    private const int NOTIFY = 150;

    public static bool enabled = true;


    public static void Fine()
    {
        if (enabled)
        {
            AndroidNativeCore.Vibrator.Vibrate(FINE);
            //Debug.Log("Vibrate FINE");
        }
    }

    public static void Fail() {
        if (enabled) {
            AndroidNativeCore.Vibrator.Vibrate(FAIL);
            //Debug.Log("Vibrate FAIL");
        }
    }

    public static void Notify() {
        if (enabled) {
            AndroidNativeCore.Vibrator.Vibrate(NOTIFY);
            //Debug.Log("Vibrate NOTIFY");
        }
    }

}
