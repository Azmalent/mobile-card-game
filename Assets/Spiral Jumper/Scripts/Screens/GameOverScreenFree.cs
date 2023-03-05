using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DiGro.Input;
using SpiralJumper.Audio;

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
        [SerializeField] private Canvas m_redDistCanvas = null;
        [Space]
        [SerializeField] private DiGro.ToggleUButton m_audioToggleButton = null;
        [SerializeField] private DiGro.ToggleUButton m_vibrationToggleButton = null;
        [SerializeField] private Button m_reviewButton = null;
        [SerializeField] private Button m_gameOverReviewButton = null;
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

        //[SerializeField] private GameObject m_testAdScreenPrefab = null;
        [Space]
        public float redDistForShowIndicator = 1;
        public float timeBeforTap = 3;
        //public int timeForContinue = 6;
        //public float freeTime = 3 * 60;
        //public float tmp_adDuration = 30;
        //public float tmp_adTimeToCancel = 5;

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


        private void Awake()
        {
            if (!m_gameOverLayer || !m_scoreText || !m_redDistCanvas
                || !m_gameOverScoreText || !m_gameLayer || !m_levelText
                || !m_gameOverLevelText || !m_gameOverBestText /*|| !m_continueButtonPrefab*/
                || !m_inputHandler || !m_gameOverTapText /*|| !m_gameOverContinueCounterText*/
                /*|| !m_continueButtonLayer*/ || !m_tapText || !m_buttonsLayer || !m_audioToggleButton
                || !m_vibrationToggleButton || !m_reviewButton || !m_gameOverReviewButton
                /*|| !m_continueFreeButtonPrefab*/ || !m_testAdScreenPrefab || !m_gameOverScoreGroup)
                Debug.LogError("Not all set in " + GetType());

            //DiGro.Check.CheckComponent<DiGro.UButton>(m_continueButtonPrefab);
            //DiGro.Check.CheckComponent<DiGro.UButton>(m_continueFreeButtonPrefab); 
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
                UpdateTapTimer();
                //UpdateContinueCounter();
            }
        }

        private void UpdateTapTimer() {
            if (!m_gameOverTapText.gameObject.activeSelf) {
                m_tapTimer += Time.deltaTime;
                if (m_tapTimer >= timeBeforTap)
                    m_gameOverTapText.gameObject.SetActive(true);
            }
        }

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
        }

        public override void OnGameOver()
        {
            int hp = 0;
            for (int i = 0; i < m_gameHearts.Count; ++i) {
                hp = m_gameHearts[i].Filled ? hp + 1 : hp;
                m_gameOverHearts[i].Filled = m_gameHearts[i].Filled;
            }

            m_isGameOver = true;
            //m_runContinueCounter = true;
            m_inputHandler.gameObject.SetActive(true);
            m_gameLayer.SetActive(false);
            m_gameOverLayer.SetActive(true);
            //m_continueButtonLayer.SetActive(true);

            m_needContinue = hp > 0;
            if (m_needContinue) {
                m_gameOverScoreGroup.SetActive(false);

                m_gameHearts[hp - 1].Filled = false;
                m_gameOverHearts[hp - 1].Fade();
                m_gameOverTapText.text = "Tap to continue";

                m_gameOverScoreGroup.SetActive(false);
            }
            else {
                m_gameOverTapText.text = "Tap to restart";

                int best = PlayerPrefs.GetInt("Best", 0);

                m_gameOverLevelText.text = m_level.ToString();
                m_gameOverScoreText.text = m_score.ToString();
                m_gameOverBestText.text = (best == 0 ? "---" : best.ToString());
                //m_gameOverContinueCounterText.text = timeForContinue.ToString();

                m_gameOverScoreGroup.SetActive(true);
                m_gameOverHeartsCanvas.SetActive(false);

                //CreateContinueButton();
            } 
        }

        public override void OnGameContinue()
        {
            m_inputHandler.gameObject.SetActive(false);
            m_gameLayer.SetActive(true);
            m_tapText.gameObject.SetActive(false);

            m_gameOverLayer.SetActive(false);
            m_gameOverTapText.gameObject.SetActive(false);

            //m_continueCounter = 0;
            //m_continueCounterTimer = 0;
            m_tapTimer = 0;

            m_isGameOver = false;

            //RemoveContinueButton();
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
            //PlayerCommands.get.Add(new PlayerCommands.Command() { action = PlayerCommands.Action.BackgroundTap });

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
        }

        private void OnRestartButtonClick()
        {
            //if (SpiralJumper.get.IsAdActive && SpiralJumper.get.NeedShowAd)
            //    ShowAd(true);
            //else
            //RestartEvent?.Invoke();

            if(m_needContinue) {
                ContinueEvent?.Invoke();
            } else {
                RestartEvent?.Invoke();
            }
        }

        //private void OnContinueButtonClick()
        //{
        //    m_tapTimer = timeBeforTap;
        //    //m_continueButtonLayer.SetActive(false);
        //    //m_runContinueCounter = false;

        //    if (SpiralJumper.get.IsAdActive) 
        //        ShowRewardedAd();
        //    else
        //        ContinueEvent?.Invoke();
        //}

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

        //private void CreateContinueButton()
        //{
        //    var prefab = m_freeTimer > 0 ? m_continueFreeButtonPrefab : m_continueButtonPrefab;

        //    var obj = Instantiate(prefab);
        //    obj.transform.parent = m_continueButtonLayer.transform;
        //    obj.transform.localPosition = Vector3.zero;

        //    m_continueButton = obj.GetComponent<DiGro.UButton>();
        //    m_continueButton.OnClick.AddListener(OnContinueButtonClick);
        //}

        //private void RemoveContinueButton()
        //{
        //    if (m_continueButton == null)
        //        return;

        //    m_continueButton.OnClick.RemoveListener(OnContinueButtonClick);
        //    Destroy(m_continueButton.gameObject);
        //    m_continueButton = null;
        //}

        //private float m_adStartTime = 0;
        //private string m_adName = "";
        //private void ShowRewardedAd()
        //{
        //    m_adStartTime = Time.time;
        //    m_adName = "R Ad";
        //    gameObject.SetActive(false);

        //    var adScreen = Instantiate(m_testAdScreenPrefab).GetComponent<TestAdScreen>();
        //    adScreen.ShowRewardedAd(tmp_adDuration, OnAdCancel, OnAdComplete);
        //}

        //private void ShowAd(bool needRestart)
        //{
        //    m_adStartTime = Time.time;
        //    m_adName = "NR Ad";
        //    gameObject.SetActive(false);

        //    var adScreen = Instantiate(m_testAdScreenPrefab).GetComponent<TestAdScreen>();
        //    adScreen.ShowAd(tmp_adDuration, tmp_adTimeToCancel, () => { 
        //        OnAdCancel();
        //        SpiralJumper.get.ResetAdTimer();
        //        if(needRestart)
        //            RestartEvent?.Invoke(); 
        //    });
        //}

        //private void OnAdCancel()
        //{
        //    float time = Time.time - m_adStartTime;
        //    GoogleSheets.SendProperty(m_adName, time.ToString(), m_level, m_score);

        //    gameObject.SetActive(true);
        //}

        //private void OnAdComplete()
        //{
        //    float time = Time.time - m_adStartTime;
        //    GoogleSheets.SendProperty(m_adName, time.ToString(), m_level, m_score);

        //    gameObject.SetActive(true);
        //    ContinueEvent?.Invoke();
        //}

        //private void PlayerCommandListener(PlayerCommands.Command command) {
        //    if (command.action == PlayerCommands.Action.BackgroundTap)
        //        OnBackgroundTap();
        //}
    }
}