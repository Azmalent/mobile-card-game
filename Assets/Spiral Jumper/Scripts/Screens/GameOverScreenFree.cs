using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DiGro.Input;
using SpiralJumper.Audio;
using Mirror;
using SR = SpiralRunner;

namespace SpiralJumper.Screens
{
    public class GameOverScreenFree : GameScreenBase
    {
        public override event Action ContinueEvent;
        public override event Action RestartEvent;
        public override event Action StartEvent;

        [SerializeField] private GameObject m_gameLayer = null;
        [SerializeField] private GameObject m_buttonsLayer = null;
        [SerializeField] private Text m_levelText = null;
        [SerializeField] private Text m_scoreText = null;
        [SerializeField] private Text m_tapText = null;
        [SerializeField] private Animator m_tapAnimator = null;
        [SerializeField] private Canvas m_redDistCanvas = null;
        [Space]
        [SerializeField] private DiGro.ToggleUButton m_audioToggleButton = null;
        [SerializeField] private DiGro.ToggleUButton m_vibrationToggleButton = null;
        [SerializeField] private Button m_reviewButton = null;
        [SerializeField] private Button m_gameOverReviewButton = null;
        [Space]
        [SerializeField] private Button m_hostButton = null;
        [SerializeField] private DiGro.ToggleUButton m_joinButton = null;
        //[Space]
        //[SerializeField] private GameObject m_continueButtonPrefab = null;
        //[SerializeField] private GameObject m_continueFreeButtonPrefab = null;
        [Space]
        [SerializeField] private GameObject m_gameOverLayer = null;
        [SerializeField] private GameObject m_gameOverScoreGroup = null;
        [SerializeField] private Text m_gameOverScoreText = null;
        [SerializeField] private Text m_gameOverLevelText = null;
        [SerializeField] private Text m_gameOverBestText = null;
        [SerializeField] private Text m_gameOverTapText = null;
        //[Space]
        //[SerializeField] private GameObject m_continueButtonLayer = null;
        //[SerializeField] private Text m_gameOverContinueCounterText = null;
        [Space]
        [SerializeField] private InputHandler m_inputHandler = null;
        [Space]
        [SerializeField] private GameObject m_testAdScreenPrefab = null;
        [Space]
        [SerializeField] private GameObject m_gameHeartsCanvas = null;
        [SerializeField] private GameObject m_gameOverHeartsCanvas = null;
        [SerializeField] private GameObject m_gameLevelGroup = null;

        //[SerializeField] private GameObject m_testAdScreenPrefab = null;
        [Space]
        public float redDistForShowIndicator = 1;
        public float timeBeforTap = 3;
        //public int timeForContinue = 6;
        //public float freeTime = 3 * 60;
        //public float tmp_adDuration = 30;
        //public float tmp_adTimeToCancel = 5;

        public override bool HasSecondPlayer { 
            get => m_hasSecondPlayer;
            set { 
                m_hasSecondPlayer = value;

                if(m_hasSecondPlayer) {
                    if (m_isGameOver)
                        m_gameOverTapText.text = "Tap to continue";

                    waitingForPlayer2 = false;
                    m_tapText.text = "Tap to start";

                    m_buttonsLayer.gameObject.SetActive(false);
                    m_tapAnimator.enabled = true;
                }
            }
        }

        [SerializeField] private bool m_hasSecondPlayer = false;

        private int m_score = 0;
        private int m_level = 1;

        //private int m_continueCounter = 0;
        //private float m_continueCounterTimer = 0;
        private float m_tapTimer = 0;
        //private float m_freeTimer = 0;

        private bool m_isGameOver = false;
        private bool m_isLongFall = false;
        //private bool m_runContinueCounter = false;

        private float m_redZoneHeight = 0;
        private float m_currentLevelHeight = 0;

        //private DiGro.UButton m_continueButton = null;

        private List<Heart> m_gameHearts;
        private List<Heart> m_gameOverHearts;

        private bool m_needContinue = false;

        private bool waitingForPlayer2 = false;

        private void Awake()
        {
            DiGro.Check.NotNull(m_gameOverLayer);
            DiGro.Check.NotNull(m_scoreText);
            DiGro.Check.NotNull(m_redDistCanvas);
            DiGro.Check.NotNull(m_gameOverScoreText);
            DiGro.Check.NotNull(m_gameLayer);
            DiGro.Check.NotNull(m_levelText);
            DiGro.Check.NotNull(m_gameOverLevelText);
            DiGro.Check.NotNull(m_gameOverBestText);
            DiGro.Check.NotNull(m_inputHandler);
            DiGro.Check.NotNull(m_gameOverTapText);
            DiGro.Check.NotNull(m_tapText);
            DiGro.Check.NotNull(m_tapAnimator);
            DiGro.Check.NotNull(m_buttonsLayer);
            DiGro.Check.NotNull(m_audioToggleButton);
            DiGro.Check.NotNull(m_vibrationToggleButton);
            DiGro.Check.NotNull(m_reviewButton);
            DiGro.Check.NotNull(m_gameOverReviewButton);
            DiGro.Check.NotNull(m_testAdScreenPrefab);
            DiGro.Check.NotNull(m_gameOverScoreGroup);
            DiGro.Check.NotNull(m_gameLevelGroup);
            DiGro.Check.NotNull(m_hostButton);
            DiGro.Check.NotNull(m_joinButton);

            DiGro.Check.CheckComponent<TestAdScreen>(m_testAdScreenPrefab);

            m_audioToggleButton.ToggleActive = AudioManager.audioEnabled;
            m_vibrationToggleButton.ToggleActive = Vibrate.enabled;

            m_gameHearts = FindHearts(m_gameHeartsCanvas);
            m_gameOverHearts = FindHearts(m_gameOverHeartsCanvas);

            if(m_gameHearts.Count != m_gameOverHearts.Count)
                Debug.LogError(GetType() + ": m_gameHearts.Count != m_gameOverHearts.Count");
        }

        private List<Heart> FindHearts(GameObject obj) {
            var hearts = obj.GetComponentsInChildren<Heart>();
            return new List<Heart>(hearts);
        }

        private void Start()
        {
            m_inputHandler.PointerDownListener += OnBackgroundTap;

            m_audioToggleButton.OnClick.AddListener(OnAudioToggleClick);
            m_vibrationToggleButton.OnClick.AddListener(OnVibrationToggleClick);

            m_reviewButton.onClick.AddListener(OnReviewButtonClick);
            m_gameOverReviewButton.onClick.AddListener(OnReviewButtonClick);

            //PlayerCommands.get.OnCommand += PlayerCommandListener;

            m_joinButton.OnClick.AddListener(OnJoinButtonClick);
        }

        private void OnDestroy()
        {
            m_inputHandler.PointerDownListener -= OnBackgroundTap;

            m_audioToggleButton.OnClick.RemoveListener(OnAudioToggleClick);
            m_vibrationToggleButton.OnClick.RemoveListener(OnVibrationToggleClick);

            m_reviewButton.onClick.RemoveListener(OnReviewButtonClick);
            m_gameOverReviewButton.onClick.RemoveListener(OnReviewButtonClick);

            //RemoveContinueButton();
        }

        private void Update()
        {
            //if(m_freeTimer > 0)
            //{
            //    m_freeTimer -= Time.deltaTime;
            //}

            if (m_isGameOver)
            {
                //UpdateTapTimer();
                //UpdateContinueCounter();
            }

            if (SpiralRunnerNetworkManager.serverInfo != null && !m_joinButton.ToggleActive)
            {
                m_joinButton.ToggleActive = true;
            }
        }

        //private void UpdateTapTimer() {
        //    if (!HasSecondPlayer) {
        //        m_tapTimer += Time.deltaTime;
        //        if (m_tapTimer >= timeBeforTap)
        //            //m_gameOverTapText.gameObject.SetActive(true);
        //            HasSecondPlayer = true;
        //    }
        //}

        //private void UpdateContinueCounter()
        //{
        //    if (m_runContinueCounter && !m_continueButton.Touched)
        //    {
        //        m_continueCounterTimer += Time.deltaTime;
        //        if (m_continueCounterTimer >= 1)
        //        {
        //            m_continueCounterTimer = 0;
        //            m_continueCounter++;

        //            m_gameOverContinueCounterText.text = (timeForContinue - m_continueCounter).ToString();

        //            if (m_continueCounter == timeForContinue)
        //            {
        //                m_continueButtonLayer.SetActive(false);
        //                m_runContinueCounter = false;

        //                if (SpiralJumper.get.IsAdActive && SpiralJumper.get.NeedShowAd)
        //                    ShowAd(false);
        //            }
        //        }
        //    }
        //}

        public override void OnGameStart()
        {
            m_inputHandler.gameObject.SetActive(true);
            m_buttonsLayer.SetActive(true);
            m_gameLayer.SetActive(true);
            m_tapText.gameObject.SetActive(true);

            m_gameOverLayer.SetActive(false);
            m_gameOverScoreGroup.SetActive(false);
            m_gameOverTapText.gameObject.SetActive(false);

            m_scoreText.text = "";
            m_gameOverScoreText.text = "";
            m_redDistCanvas.enabled = false;

            m_score = 0;
            m_level = 0;

            //m_continueCounter = 0;
            //m_continueCounterTimer = 0;
            m_tapTimer = 0;

            m_isGameOver = false;
            m_isLongFall = false;
            //m_runContinueCounter = false;

            m_redZoneHeight = 0;
            m_currentLevelHeight = 0;

            //RemoveContinueButton();

            foreach (var heart in m_gameHearts)
                heart.Filled = true;

            //m_tapText.text = "Tap To Start";

            if (!SR.SpiralRunner.get.IsNetworkGame) {
                m_joinButton.gameObject.SetActive(true);
                m_hostButton.gameObject.SetActive(true);
            }
        }

        public override void OnGameOver() {
            m_isGameOver = true;

            m_inputHandler.gameObject.SetActive(true);
            m_gameLayer.SetActive(false);
            m_gameOverLayer.SetActive(true);

            //HasSecondPlayer = false;

            m_gameOverScoreGroup.SetActive(true);
            m_gameOverLevelText.text = m_level.ToString();

            m_gameOverTapText.gameObject.SetActive(true);

            m_gameOverTapText.text = "Tap to continue";
        }

        public override void OnGameContinue()
        {
            m_inputHandler.gameObject.SetActive(false);
            m_gameLayer.SetActive(true);
            m_tapText.gameObject.SetActive(false);

            m_gameOverLayer.SetActive(false);
            m_gameOverTapText.gameObject.SetActive(false);

            m_tapTimer = 0;

            m_isGameOver = false;
        }

        public override void OnGameScoreChenged(int value)
        {
            m_score = value;
            m_scoreText.text = m_score == 0 ? "" : m_score.ToString();
        }

        public override void OnLevelChanged(int value)
        {
            m_level = value;
            m_levelText.text = value.ToString();
        }

        public override void OnLongFallBegin()
        {
            m_isLongFall = true;
            ToggleRedZoneIndicator();
        }

        public override void OnLongFallEnd()
        {
            m_isLongFall = false;
            ToggleRedZoneIndicator();
        }

        public override void OnRedZoneHeightChanged(float value)
        {
            m_redZoneHeight = value;
            ToggleRedZoneIndicator();
        }

        public override void OnCurrentLevelHeightChanged(float value)
        {
            m_currentLevelHeight = value;
            ToggleRedZoneIndicator();
        }
        
        private void ToggleRedZoneIndicator()
        {
            bool needShow = m_redZoneHeight >= m_currentLevelHeight || m_isLongFall;
            bool isShowed = m_redDistCanvas.enabled;
            if (needShow != isShowed)
                m_redDistCanvas.enabled = needShow;
        }

        private void OnBackgroundTap(DiGro.Input.EventData ev)
        {
            OnBackgroundTap();
        }

        private void OnBackgroundTap() {
            if (waitingForPlayer2) 
                return;

            if (m_tapText.gameObject.activeSelf)
                OnStartButtonClick();

            else if (m_gameOverTapText.gameObject.activeSelf)
                OnRestartButtonClick();
        }

        private void OnStartButtonClick()
        {
            m_buttonsLayer.SetActive(false);
            m_inputHandler.gameObject.SetActive(false);
            m_tapText.gameObject.SetActive(false);
            StartEvent?.Invoke();

            bool isNetworkGame = SR.SpiralRunner.get.IsNetworkGame;
            m_gameLevelGroup.SetActive(isNetworkGame);
        }

        private void OnRestartButtonClick()
        {
            if (!SR.SpiralRunner.get.IsNetworkGame || HasSecondPlayer)
                ContinueEvent?.Invoke();
        }

        public void OnHostButtonClick()
        {
            var networkManager = NetworkManager.singleton as SpiralRunnerNetworkManager;
            networkManager.CreateGame();

            m_tapText.text = "Waiting for the second player";
            waitingForPlayer2 = true;

            m_buttonsLayer.gameObject.SetActive(false);
            m_tapAnimator.enabled = false;
        }

        public void OnJoinButtonClick()
        {
            var networkManager = NetworkManager.singleton as SpiralRunnerNetworkManager;
            networkManager.JoinGame();
        }

        private void OnAudioToggleClick()
        {
            AudioManager.audioEnabled = !AudioManager.audioEnabled;
            m_audioToggleButton.ToggleActive = AudioManager.audioEnabled;
            PlayerPrefs.SetInt(Constants.PlayerPrefs.Audio, AudioManager.audioEnabled ? 1 : 0);
        }

        private void OnVibrationToggleClick()
        {
            Vibrate.enabled = !Vibrate.enabled;
            m_vibrationToggleButton.ToggleActive = Vibrate.enabled;
            PlayerPrefs.SetInt(Constants.PlayerPrefs.Vibration, Vibrate.enabled ? 1 : 0);

            if (Vibrate.enabled)
                Vibrate.Notify();
        }

        private void OnReviewButtonClick()
        {
#if UNITY_ANDROID
            string packageName = "";
            Application.OpenURL("market://details?id=" + packageName);
#endif
        }

    }
}