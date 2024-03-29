﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SJ = SpiralJumper;

namespace SpiralRunner.Controller
{
    [AddComponentMenu("Game Controller (SR)")]
    public class GameController : MonoBehaviour
    {
        private static int PlayerHeightId = Shader.PropertyToID("_Player_Height");

        public float redZoneSpeed = 1;
        public float playerStartAngleDelta = -40;
        //[Space]
        //public bool tmp_restart = false;
        //public bool tmp_gameover = false;
        [Space]
        public MeshRenderer meshRenderer;

        [Header("Achievements")]
        public List<int> BestSingleScoreValues = new List<int>() { 10, 100, 1000, 2000, 5000, 10000, 15000, 20000 };

        public int Level { get; private set; } = 1;
        public int Score { get; private set; }

        public PlayerController PlayerController => m_player;

        private SJ.View.Map m_mapView = null;
        private SJ.Model.EndlessMap m_map = null;
        [SerializeField] private PlayerController m_player = null;
        private SJ.Screens.GameScreenBase m_gameScreen = null;

        //private float m_redZoneHeight = 0;
        //private float m_redZoneTargetHeight = 0;
        //private float m_startRedZoneDistance = 0;
        //private float m_lastScoreHeight = 0;
        //private float m_lastPlayerHeight = 0;
        private float m_currentLevelHeight = 0;

        //private bool m_updatePlatforms = true;
        //private bool m_isLongFall = false;
        private bool m_gameOver = false;

        //private SJ.View.PlatformEffector m_hilightedEffector = null;
        //private int m_hilightedSector = -1;
        public SJ.View.Map MapView => m_mapView;

        private int m_nextAchieveScore = 0;

        public PlayerController LocalPlayer { 
            get => m_player;
            set { 
                m_player = value;
                if (m_player != null)
                    m_player.PlatformEnterListener += OnPlatformEnter;
            }
        }

        public PlayerController SecondPlayer;
        private float[] playerSpawnAngles = new[] { 0f, 45f };

        
        private void Awake()
        {
            DiGro.Check.NotNull(meshRenderer);

            CreateMap(SpiralRunner.get.MapParams.data);

            InitGameScreen();
            m_gameScreen.OnGameStart();

            BestSingleScoreValues.Sort();
            m_nextAchieveScore = FindNextAchieveScore();
        }

        private void OnDestroy()
        {
            RemoveGameScreen();
            if (m_player != null)
                m_player.PlatformEnterListener -= OnPlatformEnter;
        }


        private void Update()
        {
            //if(tmp_restart)
            //{
            //    tmp_restart = false;
            //    Restart();
            //    return;
            //}
            //if (tmp_gameover)
            //{
            //    tmp_gameover = false;
            //    GameOver(/*false, null*/);
            //    return;
            //}
            if (!m_gameOver)
            {
                UpdatePlatforms();
                UpdateBestSingleScore();

                //if (m_player.Position.y < m_redZoneHeight + m_player.Size / 2)
                //    GameOver(/*false, null*/);

                //if(m_mapView.NextPlatform == null)
                //    GameOver(/*true, null*/);
            }
        }

        public void SetSinglePlayer(PlayerController player) {
            LocalPlayer = player;

            LocalPlayer.transform.parent = transform;
            LocalPlayer.transform.localRotation = Quaternion.Euler(0, 0, 0);

            LocalPlayer.Init(m_mapView);
        }

        public void RemoveSinglePlayer() {
            if (LocalPlayer != null)
                LocalPlayer.PlatformEnterListener -= OnPlatformEnter;

            Destroy(LocalPlayer.gameObject);
            LocalPlayer = null;
        }

        public void SetOnlinePlayers(PlayerController local, PlayerController remote) {
            LocalPlayer = local;
            SecondPlayer = remote;

            LocalPlayer.transform.parent = transform;
            SecondPlayer.transform.parent = transform;

            float angle1 = LocalPlayer.isServer ? 0 : 45;
            float angle2 = LocalPlayer.isServer ? 45 : 0;

            LocalPlayer.transform.localRotation = Quaternion.Euler(0, angle1, 0);
            SecondPlayer.transform.localRotation = Quaternion.Euler(0, angle2, 0);

            m_gameScreen.HasSecondPlayer = true;
        }

        public void RecreateMapWithSeed(int seed) {
            var mapParams = SpiralRunner.get.MapParams.data.Clone();

            mapParams.useSeed = true;
            mapParams.seed = seed;

            DestroyMap();
            CreateMap(mapParams);
        }

        private void CreateMap(SJ.Model.MapParams mapParams) {
            m_mapView = Instantiate(SpiralRunner.get.MapViewPrefab).GetComponent<SJ.View.Map>();
            m_mapView.transform.parent = transform;

            m_map = new SJ.Model.EndlessMap(mapParams);

            m_mapView.Init(m_map);
            m_mapView.ToNextPlatform();

            Debug.Log($"CreateMap: Seed={m_map.Seed}");
        }

        private void DestroyMap() {
            Destroy(m_mapView.gameObject);
            m_map = null;
        }

        private void UpdatePlatforms()
        {
            if (m_player == null)
                return;

            var platform = m_mapView.CurrentPlatform;
            if (platform != null) {
                var playerPos = m_player.Position;
                var platformPos = platform.transform.position;
                float heightDelta = playerPos.y - platformPos.y;
                float size = m_player.Size;

                if (heightDelta > size / 2 + size * 0.2f) {
                    m_mapView.ToNextPlatform();

                    Score++;
                    m_gameScreen.OnGameScoreChenged(Score);
                }

                foreach (var sharedMaterial in meshRenderer.sharedMaterials)
                    sharedMaterial.SetFloat(PlayerHeightId, playerPos.y);
            }
        }

        private void UpdateBestSingleScore() {
            if (Score > SpiralRunner.get.BestSingleScore) {
                SpiralRunner.get.BestSingleScore = Score;

                if (Score >= m_nextAchieveScore) {
                    Firebase.LogEventUnlockAchievement($"Achieve_SingleScore_{m_nextAchieveScore}");
                    m_nextAchieveScore = FindNextAchieveScore();
                }
            }
        }

        private int FindNextAchieveScore() {
            foreach(int value in BestSingleScoreValues) {
                if(value > SpiralRunner.get.BestSingleScore)
                    return value;
            }
            return -1;
        }

        private void InitGameScreen()
        {
            if (m_gameScreen == null)
            {
                m_gameScreen = Instantiate(SpiralRunner.get.GameScreenPrefab).GetComponent<SJ.Screens.GameScreenBase>();
                m_gameScreen.transform.parent = transform;
                m_gameScreen.StartEvent += StartGame;
                m_gameScreen.RestartEvent += Restart;
                m_gameScreen.ContinueEvent += Continue;
            }
            m_gameScreen.OnLevelChanged(Level);
            //m_gameScreen.OnRedZoneDistanceChanged(m_lastPlayerHeight - m_redZoneHeight);
            //m_gameScreen.OnRedZoneHeightChanged(m_redZoneHeight);
            m_gameScreen.OnCurrentLevelHeightChanged(m_currentLevelHeight);
            m_gameScreen.OnGameScoreChenged(Score);
        }

        private void RemoveGameScreen()
        {
            if (m_gameScreen == null) /// TODO: Кажется, здесь баг
            {
                m_gameScreen.StartEvent -= StartGame;
                m_gameScreen.RestartEvent -= Restart;
                m_gameScreen.ContinueEvent -= Continue;
            }
        }

        private void OnPlatformEnter(SJ.View.PlatformEffector effector, int sector)
        {
            if (m_gameOver)
                return;

            //m_updatePlatforms = true;
            var effect = effector.GetEffect(sector);

            if (effect == SJ.Model.PlatformEffect.Level) {
                m_gameScreen.OnLevelChanged(++Level);

                effector.Platform.gameObject.SetActive(false);

                GameOver(/*false, null*/);
            }

            //if (m_player.IsFall) {
            //    if (effect == SJ.Model.PlatformEffect.Red) {
            //        GameOver(false, effector, sector);
            //        return;
            //    }
            //    var lastHeight = m_mapView.CurrentPlatform.Height;

            //    if (effect == SJ.Model.PlatformEffect.Jump) {
            //        int count = m_player.rushPlatformCount;
            //        var targetPlatform = m_mapView.GetPlatformFromCurrent(count);

            //        if (targetPlatform != null) {
            //            m_mapView.ToNextPlatform(count);

            //            /// TODO: изменить высоту зоны и счет.
            //            /// Сначала для как для обычной платформы, потом подождать
            //            /// и для всех пропущенных платформ.
            //            return;
            //        }
            //    }
            //    if (effector.Platform == m_mapView.NextPlatform) {
            //        m_mapView.ToNextPlatform();

            //        var newHeight = m_mapView.CurrentPlatform.Height;
            //        m_redZoneTargetHeight += (newHeight - lastHeight) * redZoneSpeed;

            //        if (m_lastScoreHeight < newHeight) {
            //            m_lastScoreHeight = newHeight;
            //            Score++;
            //            m_gameScreen.OnGameScoreChenged(Score);
            //        }
            //    }
            //}
            //if (m_isLongFall)
            //    LongFallEnd();
        }

        //private void LongFallBegin()
        //{
        //    if (m_mapView.CurrentPlatform.Type != SJ.Model.PlatformType.SaveRing)
        //    {
        //        m_mapView.ToSavePlatform();
        //        m_gameScreen.OnLongFallBegin();
        //        //m_isLongFall = true;
        //        m_updatePlatforms = false;
        //    }
        //}

        //private void LongFallEnd()
        //{
        //    m_gameScreen.OnLongFallEnd();
        //    //m_isLongFall = false;
        //    m_updatePlatforms = true;
        //}

        private void StartGame()
        {
            if (m_player == null)
                return;

            m_player.OnGameStart();
        }

        private void GameOver(/*bool success, SJ.View.PlatformEffector effector, int sector = -1*/)
        {
            m_gameOver = true;

            //int lastBest = PlayerPrefs.GetInt("Best", 0);
            //if (lastBest < Score)
            //    PlayerPrefs.SetInt("Best", Score);

            //int lastCoins = PlayerPrefs.GetInt("Coins", 0);
            //PlayerPrefs.SetInt("Coins", lastCoins + m_lastLevel);

            //if (effector != null)
            //{
            //    effector.SetHilight(sector, true);
            //    m_hilightedEffector = effector;
            //    m_hilightedSector = sector;
            //}

            m_gameScreen.OnGameOver();

            if (m_player != null)
                m_player.OnGameOver();
        }

        public void Continue()
        {
            var current = m_mapView.CurrentPlatform;
            var next = m_mapView.NextPlatform;

            if (current != null && current.Type != SJ.Model.PlatformType.SaveRing) current.Visible = false;
            if (next != null && next.Type != SJ.Model.PlatformType.SaveRing) next.Visible = false;

            //m_updatePlatforms = false;

            //if (m_hilightedEffector != null)
            //{
            //    m_hilightedEffector.SetHilight(m_hilightedSector, false);
            //    m_hilightedEffector = null;
            //    LongFallBegin();
            //}

            //float targetHeight = m_mapView.CurrentPlatform.Height - m_startRedZoneDistance;
            //if (m_redZoneHeight > targetHeight)
            //{
            //    m_redZoneHeight = targetHeight;
            //    m_redZoneTargetHeight = m_redZoneHeight;
            //    m_mapView.SetRedHeight(m_redZoneHeight);
            //}

            // Destroy(m_gameScreen.gameObject);
            InitGameScreen();
            m_gameScreen.OnGameContinue();

            if (m_player != null)
                m_player.OnGameContinue();

            m_gameOver = false;
        }

        public void Restart()
        {
            Destroy(gameObject);
            Instantiate(SpiralRunner.get.GameControllerPrefab);
        }

    }
}
