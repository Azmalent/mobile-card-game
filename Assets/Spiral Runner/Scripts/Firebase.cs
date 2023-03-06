using System;
using UnityEngine;

using FA = Firebase.Analytics;

namespace SpiralRunner {

    public class Firebase : DiGro.SingletonMono<Firebase> {

        public static global::Firebase.FirebaseApp app { get; private set; } = null;

        public static bool Ready => app != null;


        private void Start() {
            global::Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == global::Firebase.DependencyStatus.Available) {
                    app = global::Firebase.FirebaseApp.DefaultInstance;
                }
                else {// Firebase Unity SDK is not safe to use here.
                    Debug.LogError(string.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
            });
        }

        static public void LogPostScoreEvent(int score) {
            FA.FirebaseAnalytics.LogEvent(
                FA.FirebaseAnalytics.EventPostScore,
                FA.FirebaseAnalytics.ParameterScore,
                score
            );
        }

        static public void LogEventUnlockAchievement(string achievementId) {
            FA.FirebaseAnalytics.LogEvent(
                FA.FirebaseAnalytics.EventUnlockAchievement,
                FA.FirebaseAnalytics.ParameterAchievementId,
                achievementId
            );
        }

    }

}

