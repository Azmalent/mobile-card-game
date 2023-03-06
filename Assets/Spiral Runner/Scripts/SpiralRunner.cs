using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using SpiralJumper.Audio;
using SpiralJumper;


namespace SpiralRunner
{
    public class SpiralRunner : DiGro.SingletonMono<SpiralRunner>
    {

        public Guid SessionID { get; private set; }

        [SerializeField] private MapParams m_mapParams = null;

        [SerializeField] private GameObject m_mapViewPrefab = null;
        [SerializeField] private GameObject m_gameControllerPrefab = null;
        [SerializeField] private GameObject m_playerControllerPrefab = null;
        [SerializeField] private GameObject m_gameScreenPrefab = null;
        //[SerializeField] private GameObject m_gameOverScreenPrefab = null;

        [SerializeField] private bool useRandomPlayer = false;
        [SerializeField] private List<GameObject> m_playerPrefabList = new List<GameObject>();

        public Color activePlatformColor;
        public Color inactivePlatformColor;
        public Color savePlatformColor;

        public Text m_debugText;

        [Space]
        [SerializeField] private bool m_logging = false;
        [SerializeField] private bool m_sendStat = false; 
        [SerializeField] private bool m_isAdActive = true;
        [Space]
        [SerializeField] private float m_timeBetweenAd = 60;
        
        public SpiralJumper.MapParams MapParams => m_mapParams;
        public GameObject MapViewPrefab => m_mapViewPrefab;
        public GameObject GameControllerPrefab => m_gameControllerPrefab;
        public GameObject PlayerControllerPrefab
        {
            get
            {
                if (useRandomPlayer)
                    return m_playerPrefabList[UnityEngine.Random.Range(0, m_playerPrefabList.Count)];
                return m_playerControllerPrefab;
            }
        }

        public GameObject GameScreenPrefab => m_gameScreenPrefab;
        //public GameObject GameOverScreenPrefab => m_gameOverScreenPrefab;

        public Controller.GameController GameController { get; private set; }

        public bool NeedShowAd => m_adTimer <= 0;
        public bool IsAdActive => m_isAdActive;
        public bool NeedSendStat => m_sendStat;
        public float PlayTime => Time.time - m_startTime;

        public float RevardedAdTime { get; set; }
        public float NotRevardedAdTime { get; set; }

        public int BestSingleScore { get; set; }

        public bool Debug { get; private set; }

        private int m_frameCount = 0;
        private float m_currentTime = 0;
        private float m_startTime = 0;
        private long m_totalFrames = 0;

        private StreamWriter m_logFile = null;
        private LinkedList<string> m_debugAdditiveInfo = new LinkedList<string>();
        private string m_baseDebugInfo = "";
        private bool m_needUpdateDebugText = false;

        private float m_adTimer = 0;


        private void Awake()
        {
            SessionID = Guid.NewGuid();

            DiGro.Check.NotNull(m_mapParams);
            DiGro.Check.NotNull(m_debugText);

            DiGro.Check.CheckComponent<SpiralJumper.View.Map>(m_mapViewPrefab);
            DiGro.Check.CheckComponent<Controller.GameController>(m_gameControllerPrefab);
            DiGro.Check.CheckComponent<Controller.PlayerController>(m_playerControllerPrefab);
            DiGro.Check.CheckComponent<SpiralJumper.Screens.GameScreenBase>(m_gameScreenPrefab);

            if (Camera.main != null)
                Camera.main.gameObject.SetActive(false);

            Application.targetFrameRate = 60;

            ReadPrefs();

            if (m_logging)
            {
                try
                {
                    var logFileFullName = "";
                    //m_logFile = File.CreateText(logFileFullName + DateTime.Now.ToString().Replace(":", "-") + ".tvs");
                }
                catch (Exception ex)
                {
                    m_logFile = null;
                }
            }
            //if (playerActionsCommand == PlayerActionsCommand.Write)
            //    m_playerActions = new Controller.PlayerActions();

            //if(playerActionsCommand == PlayerActionsCommand.Use)
            //    DiGro.Check.NotNull(m_playerActions);

            Debug = Application.version.Contains(".d");

            m_debugText.gameObject.SetActive(Debug);
        }

        private void Start()
        {
            m_startTime = Time.time;
            GameController = Instantiate(GameControllerPrefab).GetComponent<Controller.GameController>();
            GameController.LocalPlayer = GameController.SpawnPlayer(0);
            //GameController.SecondPlayer = GameController.SpawnPlayer(1);
        }

        public void RestartWithSeed(int seed)
        {
            Destroy(GameController.gameObject);

            m_mapParams.data.useSeed = true;
            m_mapParams.data.seed = seed;

            m_startTime = Time.time;
            GameController = Instantiate(GameControllerPrefab).GetComponent<Controller.GameController>();
            GameController.LocalPlayer = GameController.SpawnPlayer(1);
            GameController.SecondPlayer = GameController.SpawnPlayer(0);
        }

        private void Update()
        {
            if (m_adTimer > 0)
                m_adTimer -= Time.deltaTime;

            m_frameCount++;
            m_currentTime += Time.deltaTime;
            if (m_currentTime >= 1)
            {
                m_totalFrames += m_frameCount;

                int fps = m_frameCount;
                int avFps = (int)(m_totalFrames / (Time.time - m_startTime));

                m_baseDebugInfo = "[" + avFps.ToString() + "] " + "[" + fps.ToString() + "] " + Application.version;
                m_needUpdateDebugText = true;

                m_frameCount = 0;
                m_currentTime = 0;
            }
        }

        private void LateUpdate()
        {
            //if (m_logFile != null)
            //    m_logFile.WriteLine(m_frameCount.ToString() + "\t" + Time.deltaTime.ToString());
            if (m_needUpdateDebugText) {
                m_needUpdateDebugText = false;

                string str = m_baseDebugInfo;
                foreach (var additiveInfo in m_debugAdditiveInfo)
                    str += " " + additiveInfo;

                m_debugText.text = str;
            }
        }

        private void OnDestroy()
        {
            WritePrefs();
            if (m_logFile != null)
                m_logFile.Close();

            //GoogleSheets.SendProperty("Exit", PlayTime.ToString());

            //if (playerActionsCommand == PlayerActionsCommand.Write)
            //{
            //    var path = "Assets"; // Application.dataPath;
            //    path += "/Spiral Jumper Prototype/Saves/Player Actions ";
            //    path += DateTime.Now.ToString().Replace(":", "-");
            //    path += ".asset";

            //    AssetDatabase.CreateAsset(playerActions, path);
            //    AssetDatabase.SaveAssets();
            //}
        }


        public void ResetAdTimer() => m_adTimer = m_timeBetweenAd;

        private void ReadPrefs()
        {
            AudioManager.audioEnabled = PlayerPrefs.GetInt(Constants.PlayerPrefs.Audio, 1) > 0 ? true : false;
            Vibrate.enabled = PlayerPrefs.GetInt(Constants.PlayerPrefs.Vibration, 1) > 0 ? true : false;
            BestSingleScore = PlayerPrefs.GetInt(Constants.PlayerPrefs.BestSingleScore, 0);
        }

        private void WritePrefs()
        {
            PlayerPrefs.SetInt(Constants.PlayerPrefs.Audio, AudioManager.audioEnabled ? 1 : 0);
            PlayerPrefs.SetInt(Constants.PlayerPrefs.Vibration, Vibrate.enabled ? 1 : 0);
            PlayerPrefs.SetInt(Constants.PlayerPrefs.BestSingleScore, BestSingleScore);
        }

        public void Log(string str)
        {
            if (m_logFile != null)
                m_logFile.WriteLine(str);
        }

        public DebugInfo CreateDebugInfo() => new DebugInfo(m_debugAdditiveInfo.AddLast(""));


        public class DebugInfo
        {
            public string Value
            {
                get => m_node.Value;
                set
                {
                    m_node.Value = value;
                    get.m_needUpdateDebugText = true;
                }
            }

            private LinkedListNode<string> m_node;

            public DebugInfo(LinkedListNode<string> node) => m_node = node;

            public void Remove()
            {
                m_node.List.Remove(m_node);
                var spiralJumper = get;
                if (spiralJumper != null)
                    spiralJumper.m_needUpdateDebugText = true;
            }
        }
    }
}