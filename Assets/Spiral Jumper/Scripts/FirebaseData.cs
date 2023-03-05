//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace SpiralJumper {

//    public class FirebaseData : DiGro.SingletonMono<FirebaseData> {

//        public static Firebase.FirebaseApp app { get; private set; } = null;

//        public static bool Ready => app != null;


//        private void Start() {
//            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
//                var dependencyStatus = task.Result;
//                if (dependencyStatus == Firebase.DependencyStatus.Available) {
//                    app = Firebase.FirebaseApp.DefaultInstance;
//                }
//                else {// Firebase Unity SDK is not safe to use here.
//                    Debug.LogError(String.Format(
//                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//                }
//            });
//        }

//        private void Update() {
//        }

//    }

//}

