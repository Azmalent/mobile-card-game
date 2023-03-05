using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DiGro.Input;
using SpiralJumper.Audio;

namespace SpiralJumper.Screens
{
    public class GameOverScreenWithAdTest : GameScreenBase
    {
        public override event Action ContinueEvent;
        public override event Action RestartEvent;
        public override event Action StartEvent;

        [SerializeField] private Button m_gameOverReviewButton = null;
        [Space]
        [SerializeField] private GameObject m_continueButtonPrefab = null;
        [SerializeField] private GameObject m_continueFreeButtonPrefab = null;
        [Space]
        [SerializeField] private GameObject m_gameOverLayer = null;
        [SerializeField] private Text m_gameOverScoreText = null;
        [SerializeField] private Text m_gameOverLevelText = null;
        [SerializeField] private Text m_gameOverBestText = null;
        [SerializeField] private Text m_gameOverTapText = null;
        [Space]
        [SerializeField] private GameObject m_continueButtonLayer = null;
        [SerializeField] private Text m_gameOverContinueCounterText = null;
        [Space]
        [SerializeField] private InputHandler m_inputHandler = null;
        [Space]
        [SerializeField] private GameObject m_testAdScreenPrefab = null;
        [Space]
        public float timeBeforTap = 3;
        public int timeForContinue = 6;
        public float freeTime = 3 * 60;
        public float tmp_adDuration = 30;
        public float tmp_adTimeToCancel = 5;

        private int m_score = 0;
        private int m_level = 1;

        private int m_continueCounter = 0;
        private float m_continueCounterTimer = 0;
        private float m_tapTimer = 0;
        private float m_freeTimer = 0;

        private bool m_isGameOver = false;
        private bool m_runContinueCounter = false;

        private DiGro.UButton m_continueButton = null;


        private void Awake()
        {
            if (!m_gameOverLayer 
                || !m_gameOverScoreText
                || !m_gameOverLevelText || !m_gameOverBestText || !m_continueButtonPrefab
                || !m_inputHandler || !m_gameOverTapText || !m_gameOverContinueCounterText
                || !m_continueButtonLayer
                || !m_gameOverReviewButton
                || !m_continueFreeButtonPrefab || !m_testAdScreenPrefab)
                Debug.LogError("Not all set in " + GetType());

            DiGro.Check.CheckComponent<DiGro.UButton>(m_continueButtonPrefab);
            DiGro.Check.CheckComponent<DiGro.UButton>(m_continueFreeButtonPrefab); 
            DiGro.Check.CheckComponent<TestAdScreen>(m_testAdScreenPrefab);

        }

        private void Start()
        {
            m_inputHandler.PointerDownListener += OnBackgroundTap;
            m_gameOverReviewButton.onClick.AddListener(OnReviewButtonClick);
        }

        private void OnDestroy()
        {
            m_inputHandler.PointerDownListener -= OnBackgroundTap;
            m_gameOverReviewButton.onClick.RemoveListener(OnReviewButtonClick);

            RemoveContinueButton();
        }

        private void Update()
        {
            if (m_isGameOver)
            {
                UpdateTapTimer();
                UpdateContinueCounter();
            }
        }

        private void UpdateTapTimer()
        {
            if (!m_gameOverTapText.gameObject.activeSelf)
            {
                m_tapTimer += Time.deltaTime;
                if (m_tapTimer >= timeBeforTap)
                    m_gameOverTapText.gameObject.SetActive(true);
            }
        }

        private void UpdateContinueCounter()
        {
            if (m_runContinueCounter && !m_continueButton.Touched)
            {
                m_continueCounterTimer += Time.deltaTime;
                if (m_continueCounterTimer >= 1)
                {
                    m_continueCounterTimer = 0;
                    m_continueCounter++;

                    m_gameOverContinueCounterText.text = (timeForContinue - m_continueCounter).ToString();

                    if (m_continueCounter == timeForContinue)
                    {
                        m_continueButtonLayer.SetActive(false);
                        m_runContinueCounter = false;

                        if (SpiralJumper.get.IsAdActive && SpiralJumper.get.NeedShowAd)
                            ShowAd(false);
                    }
                }
            }
        }

        public override void OnGameStart()
        {
            m_inputHandler.gameObject.SetActive(false);

            m_gameOverLayer.SetActive(false);
            m_gameOverTapText.gameObject.SetActive(false);

            m_gameOverScoreText.text = "";

            m_score = 0;
            m_level = 0;

            m_continueCounter = 0;
            m_continueCounterTimer = 0;
            m_tapTimer = 0;

            m_isGameOver = false;
            m_runContinueCounter = false;

            RemoveContinueButton();
        }

        public override void OnGameOver()
        {
            m_isGameOver = true;
            m_runContinueCounter = true;
            m_inputHandler.gameObject.SetActive(true);
            m_gameOverLayer.SetActive(true);
            m_continueButtonLayer.SetActive(true);

            int best = PlayerPrefs.GetInt("Best", 0);

            m_gameOverLevelText.text = m_level.ToString();
            m_gameOverScoreText.text = m_score.ToString();
            m_gameOverBestText.text = (best == 0 ? "---" : best.ToString());
            m_gameOverContinueCounterText.text = timeForContinue.ToString();

            CreateContinueButton();
        }

        public override void OnGameContinue()
        {
            m_inputHandler.gameObject.SetActive(false);

            m_gameOverLayer.SetActive(false);
            m_gameOverTapText.gameObject.SetActive(false);

            m_continueCounter = 0;
            m_continueCounterTimer = 0;
            m_tapTimer = 0;

            m_isGameOver = false;

            RemoveContinueButton();
        }

        public override void OnGameScoreChenged(int value)
        {
            m_score = value;
        }

        public override void OnLevelChanged(int value)
        {
            m_level = value;
        }
                
        private void OnBackgroundTap(DiGro.Input.EventData ev)
        {
            if (m_gameOverTapText.gameObject.activeSelf)
                OnRestartButtonClick();
        }

        private void OnRestartButtonClick()
        {
            if (SpiralJumper.get.IsAdActive && SpiralJumper.get.NeedShowAd)
                ShowAd(true);
            else
                RestartEvent?.Invoke();
        }

        private void OnContinueButtonClick()
        {
            m_tapTimer = timeBeforTap;
            m_continueButtonLayer.SetActive(false);
            m_runContinueCounter = false;

            if (SpiralJumper.get.IsAdActive) 
                ShowRewardedAd();
            else
                ContinueEvent?.Invoke();
        }

        private void OnReviewButtonClick()
        {
#if UNITY_ANDROID
            string packageName = "";
            Application.OpenURL("market://details?id=" + packageName);
#endif
        }

        private void CreateContinueButton()
        {
            var prefab = m_freeTimer > 0 ? m_continueFreeButtonPrefab : m_continueButtonPrefab;

            var obj = Instantiate(prefab);
            obj.transform.parent = m_continueButtonLayer.transform;
            obj.transform.localPosition = Vector3.zero;

            m_continueButton = obj.GetComponent<DiGro.UButton>();
            m_continueButton.OnClick.AddListener(OnContinueButtonClick);
        }

        private void RemoveContinueButton()
        {
            if (m_continueButton == null)
                return;

            m_continueButton.OnClick.RemoveListener(OnContinueButtonClick);
            Destroy(m_continueButton.gameObject);
            m_continueButton = null;
        }

        private float m_adStartTime = 0;
        private string m_adName = "";
        private void ShowRewardedAd()
        {
            m_adStartTime = Time.time;
            m_adName = "R Ad";
            gameObject.SetActive(false);

            var adScreen = Instantiate(m_testAdScreenPrefab).GetComponent<TestAdScreen>();
            adScreen.ShowRewardedAd(tmp_adDuration, OnAdCancel, OnAdComplete);
        }

        private void ShowAd(bool needRestart)
        {
            m_adStartTime = Time.time;
            m_adName = "NR Ad";
            gameObject.SetActive(false);

            var adScreen = Instantiate(m_testAdScreenPrefab).GetComponent<TestAdScreen>();
            adScreen.ShowAd(tmp_adDuration, tmp_adTimeToCancel, () => { 
                OnAdCancel();
                SpiralJumper.get.ResetAdTimer();
                if(needRestart)
                    RestartEvent?.Invoke(); 
            });
        }

        private void OnAdCancel()
        {
            float time = Time.time - m_adStartTime;
            GoogleSheets.SendProperty(m_adName, time.ToString(), m_level, m_score);

            gameObject.SetActive(true);
        }

        private void OnAdComplete()
        {
            float time = Time.time - m_adStartTime;
            GoogleSheets.SendProperty(m_adName, time.ToString(), m_level, m_score);

            gameObject.SetActive(true);
            ContinueEvent?.Invoke();
        }

        public override void OnLongFallBegin() { }

        public override void OnLongFallEnd() { }
    }
}