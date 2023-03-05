using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DiGro.Input;
using SpiralJumper.Audio;

namespace SpiralJumper.Screens
{
    public class GameScreen : GameScreenBase
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
        [Space]
        [SerializeField] private InputHandler m_inputHandler = null;
 
        private int m_score = 0;
        private int m_level = 1;

        private bool m_isGameOver = false;
        private bool m_isLongFall = false;

        private float m_redZoneHeight = 0;
        private float m_currentLevelHeight = 0;


        private void Awake()
        {
            if (!m_scoreText || !m_redDistCanvas || !m_gameLayer || !m_levelText
                || !m_inputHandler || !m_tapText || !m_buttonsLayer || !m_audioToggleButton
                || !m_vibrationToggleButton || !m_reviewButton)
                Debug.LogError("Not all set in " + GetType());

            m_audioToggleButton.ToggleActive = AudioManager.audioEnabled;
            m_vibrationToggleButton.ToggleActive = Vibrate.enabled;
        }

        private void Start()
        {
            m_inputHandler.PointerDownListener += OnBackgroundTap;

            m_audioToggleButton.OnClick.AddListener(OnAudioToggleClick);
            m_vibrationToggleButton.OnClick.AddListener(OnVibrationToggleClick);

            m_reviewButton.onClick.AddListener(OnReviewButtonClick);
        }

        private void OnDestroy()
        {
            m_inputHandler.PointerDownListener -= OnBackgroundTap;

            m_audioToggleButton.OnClick.RemoveListener(OnAudioToggleClick);
            m_vibrationToggleButton.OnClick.RemoveListener(OnVibrationToggleClick);

            m_reviewButton.onClick.RemoveListener(OnReviewButtonClick);
        }

        public override void OnGameStart()
        {
            m_inputHandler.gameObject.SetActive(true);
            m_buttonsLayer.SetActive(true);
            m_gameLayer.SetActive(true);
            m_tapText.gameObject.SetActive(true);

            m_scoreText.text = "";
            m_redDistCanvas.enabled = false;

            m_score = 0;
            m_level = 0;

            //m_continueCounter = 0;
            //m_continueCounterTimer = 0;
            //m_tapTimer = 0;

            m_isGameOver = false;
            m_isLongFall = false;
            //m_runContinueCounter = false;

            m_redZoneHeight = 0;
            m_currentLevelHeight = 0;
        }

        public override void OnGameOver()
        {
            m_isGameOver = true;
            //m_runContinueCounter = true;
            m_inputHandler.gameObject.SetActive(true);
            m_gameLayer.SetActive(false);
        }

        public override void OnGameContinue()
        {
            m_inputHandler.gameObject.SetActive(false);
            m_gameLayer.SetActive(true);
            m_tapText.gameObject.SetActive(false);

            //m_continueCounter = 0;
            //m_continueCounterTimer = 0;
            //m_tapTimer = 0;

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
            if (m_tapText.gameObject.activeSelf)
                OnStartButtonClick();
        }

        private void OnStartButtonClick()
        {
            m_buttonsLayer.SetActive(false);
            m_inputHandler.gameObject.SetActive(false);
            m_tapText.gameObject.SetActive(false);
            StartEvent?.Invoke();
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