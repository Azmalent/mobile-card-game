//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//namespace SpiralJumper {
//    public class PlayerCommands : DiGro.SingletonMono<PlayerCommands> {

//        public enum Action {
//            Fall,
//            BackgroundTap,
//        }

//        [Serializable]
//        public class Command {
//            public PlayerCommands.Action action;
//            public float time;
//            public long tick;

//            public Vector3 vec1;
//            public Vector3 vec2;
//        }

//        public enum Usege { None, Use, Write }

//        public event System.Action<PlayerCommands.Command> OnCommand;

//        public Usege usege = Usege.None;
//        [SerializeField] private PlayerCommandList m_usedData = null;
//        [Space]
//        public string savePath = "Assets/Spiral Jumper/Saves";
//        [Space]
//        [SerializeField] private bool m_save = false;

//        [SerializeField] private bool m_capture = false;

//        [Space]
//        public string captureName = "";
//        public List<int> captureList = new List<int>();
//        private int m_captureIndex = 0;

//        private PlayerCommandList m_data;
//        private int m_index = 0;
//        private long m_tick = 0;


//        private void Awake() {
//            if (usege == Usege.Use)
//                m_data = m_usedData;

//            else if(usege == Usege.Write)
//                m_data = ScriptableObject.CreateInstance<PlayerCommandList>();
//        }

//        private void Update() {
//            m_tick++;

//            if (usege == Usege.Use) {
//                if(m_index < m_data.commands.Count) {
//                    if (m_data.commands[m_index].tick == m_tick) {
//                        OnCommand?.Invoke(m_data.commands[m_index]);
//                        m_index++;
//                    }
//                    //if (Time.realtimeSinceStartup >= m_data.commands[m_index].time) {
//                    //    OnCommand?.Invoke(m_data.commands[m_index]);
//                    //    m_index++;
//                    //}
//                }
//            }
//            if(usege == Usege.Write) {
//                if (m_save) {
//                    Save();
//                    m_save = false;
//                }
//            }
//            if (m_capture) {
//                m_capture = false;
//                CaprureScreenshot();
//            }
//        }

//        private void LateUpdate() {
//            if (usege == Usege.Use) {
//                if (m_captureIndex < captureList.Count) {
//                    if (captureList[m_captureIndex] == m_tick) {
//                        m_captureIndex++;
//                        CaprureScreenshot();
//                    }
//                }
//            }
//        }

//        private void Save() {
//            if (usege != Usege.Write)
//                return;

//#if UNITY_EDITOR

//            var time = DateTime.Now.ToString().Replace(":", "-");
//            var path = savePath + "/" + "PlayerCommands " + time + ".asset";

//            AssetDatabase.CreateAsset(m_data, path);
//            AssetDatabase.SaveAssets();
//#endif 

//        }


//        public void Add(PlayerCommands.Command command) {
//            if (usege != Usege.Write)
//                return;

//            command.time = Time.realtimeSinceStartup;
//            command.tick = m_tick;

//            m_data.commands.Add(command);
//        }

//        private void CaprureScreenshot() {
//            var res = Screen.currentResolution;
//            var filename = $"{Application.productName} {captureName} {m_tick} {res.width}X{res.height}.png";

//            ScreenCapture.CaptureScreenshot("C:/Users/Dmitry/Desktop/SpiralJumper Capture/Screenshots/" + filename);
//        }

//    }
//}
