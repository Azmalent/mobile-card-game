using System;
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
        public float redZoneSpeed = 1;
        public float playerStartAngleDelta = -40;
        [Space]
        public bool tmp_restart = false;
        public bool tmp_gameover = false;

        public int Level { get; private set; } = 1;
        public int Score { get; private set; }

        public PlayerController PlayerController => m_player;

        private SJ.View.Map m_mapView = null;
        private SJ.Model.EndlessMap m_map = null;
        private PlayerController m_player = null;
        private SJ.Screens.GameScreenBase m_gameScreen = null;

        private float m_redZoneHeight = 0;
        private float m_redZoneTargetHeight = 0;
        private float m_startRedZoneDistance = 0;
        private float m_lastScoreHeight = 0;
        private float m_lastPlayerHeight = 0;
        private float m_currentLevelHeight = 0;

        private bool m_updatePlatforms = true;
        private bool m_isLongFall = false;
        private bool m_gameOver = false;

        private SJ.View.PlatformEffector m_hilightedEffector = null;
        private int m_hilightedSector = -1;
        public SJ.View.Map MapView => m_mapView;

        public PlayerController LocalPlayer { get => m_player; set => m_player = value; }
        public PlayerController SecondPlayer { get; set; }
        private float[] playerSpawnAngles = new[] { 0f, 180f };

        private void Awake()
        {
            m_mapView = Instantiate(SpiralRunner.get.MapViewPrefab).GetComponent<SJ.View.Map>();
            m_mapView.transform.parent = transform;

            var mapParams = SpiralRunner.get.MapParams;
            m_map = new SJ.Model.EndlessMap(mapParams.data);
            
            m_mapView.Init(m_map);

            var playerPrefab = SpiralRunner.get.PlayerControllerPrefab;

            if (m_map.LevelsCount > 0)
            {
                var level = m_map.GetLevel(0);

                float firstAngle = level.FirstChank.platforms.Count < 2 ? 0 : level.FirstChank.platforms[1].angle;
                var rotation = Quaternion.Euler(0, firstAngle + playerStartAngleDelta, 0);
                playerPrefab.transform.localRotation = rotation;

                m_redZoneHeight = level.beginHeight - level.endHeight;
                m_redZoneTargetHeight = m_redZoneHeight;
                m_startRedZoneDistance = Mathf.Abs(m_redZoneHeight);
            }

            m_mapView.SetRedHeight(m_redZoneHeight);

            InitGameScreen();
            m_gameScreen.OnGameStart();

            m_player = SpawnPlayer(0);
            //m_player.PlatformEnterListener += OnPlatformEnter;

            StartCoroutine(UpdateRedZoneDistance());
        }

        public PlayerController SpawnPlayer(int playerId)
        {
            var playerPrefab = SpiralRunner.get.PlayerControllerPrefab;
            var player = Instantiate(playerPrefab).GetComponent<PlayerController>();

            player.transform.parent = transform;
            player.transform.localRotation = Quaternion.Euler(0, playerSpawnAngles[playerId], 0);
            //TODO: player color?

            player.Init(m_mapView);

            return player;
        }

        private void OnDestroy()
        {
            RemoveGameScreen();
            //if (m_player != null)
            //    m_player.PlatformEnterListener -= OnPlatformEnter;
        }


        private void Update()
        {
            if(tmp_restart)
            {
                tmp_restart = false;
                Restart();
                return;
            }
            if (tmp_gameover)
            {
                tmp_gameover = false;
                GameOver(false, null);
                return;
            }
            if (!m_gameOver)
            {
                UpdatePlatforms();
                UpdateRedZone();
                //UpdatePlayer();

                if (m_player.Position.y < m_redZoneHeight + m_player.Size / 2)
                    GameOver(false, null);

                if(m_mapView.NextPlatform == null)
                    GameOver(true, null);
            }
            //else if (Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //    Continue();
            //}
        }


        private void UpdateRedZone()
        {
            if(m_redZoneHeight != m_redZoneTargetHeight)
            {
                float height = Mathf.MoveTowards(m_redZoneHeight, m_redZoneTargetHeight, Time.deltaTime);
                m_redZoneHeight = height;
                m_mapView.SetRedHeight(m_redZoneHeight);
                m_gameScreen.OnRedZoneHeightChanged(m_redZoneHeight);
            }
        }

        //private void UpdatePlayer()
        //{
        //    if (m_lastPlayerHeight != m_player.Position.y)
        //    {
        //        m_lastPlayerHeight = m_player.Position.y;
        //        m_gameScreen.OnRedZoneDistationChanged(m_lastPlayerHeight - m_redZoneHeight);
        //    }
        //}

        private IEnumerator UpdateRedZoneDistance()
        {
            float lastHeight = m_player.Position.y;
            while(true)
            {
                if (lastHeight != m_player.Position.y)
                {
                    lastHeight = m_player.Position.y;
                    m_gameScreen.OnRedZoneDistanceChanged(lastHeight - m_redZoneHeight);
                }
                yield return new WaitForSeconds(1f/24f);
            }
        }

        private void UpdatePlatforms()
        {
            var platform = m_mapView.NextPlatform;
            if (platform != null) {
                var playerPos = m_player.Position;
                var platformPos = platform.transform.position;
                float heightDelta = playerPos.y - platformPos.y;
                float size = m_player.Size;

                if (heightDelta > size / 2 + size * 0.2f) {
                    m_mapView.ToNextPlatform();
                }
            }

            //var platform = m_mapView.NextPlatform;
            //if (platform != null)
            //{
            //    var playerPos = m_player.Position;
            //    var platformPos = platform.transform.position;
            //    float heightDelta = playerPos.y - platformPos.y;
            //    float size = m_player.Size;

            //    if (heightDelta > size / 2 + size * 0.2f)
            //    {
            //        if (platform.Type == SJ.Model.PlatformType.SaveRing && m_mapView.CurrentLevel + 1 == Level)
            //        {
            //            Level++;
            //            m_currentLevelHeight = platform.Height;
            //            m_gameScreen.OnLevelChanged(Level);
            //            m_gameScreen.OnCurrentLevelHeightChanged(m_currentLevelHeight);
            //        }
            //        if (m_updatePlatforms)
            //        {
            //            if (!platform.Visible)
            //                platform.Visible = true;
            //        }
            //    }
            //    if (heightDelta <= 0)
            //    {
            //        if (platform.Visible)
            //            platform.Visible = false;
            //    }
            //}
            //var currentPlatform = m_mapView.CurrentPlatform;
            //if (currentPlatform != null)
            //{
            //    var playerPos = m_player.Position;
            //    var platformPos = currentPlatform.transform.position;

            //    if (playerPos.y < platformPos.y)
            //        LongFallBegin();
            //}
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
            m_gameScreen.OnRedZoneDistanceChanged(m_lastPlayerHeight - m_redZoneHeight);
            m_gameScreen.OnRedZoneHeightChanged(m_redZoneHeight);
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

            m_updatePlatforms = true;
            var effect = effector.GetEffect(sector);


            if (m_player.IsFall)
            {
                if (effect == SJ.Model.PlatformEffect.Red)
                {
                    GameOver(false, effector, sector);
                    return;
                }
                var lastHeight = m_mapView.CurrentPlatform.Height;

                if (effect == SJ.Model.PlatformEffect.Jump) {
                    int count = m_player.rushPlatformCount;
                    var targetPlatform = m_mapView.GetPlatformFromCurrent(count);

                    if(targetPlatform != null) {
                        m_mapView.ToNextPlatform(count);

                        /// TODO: изменить высоту зоны и счет.
                        /// Сначала для как для обычной платформы, потом подождать
                        /// и для всех пропущенных платформ. 
                        return;
                    }
                }
                if (effector.Platform == m_mapView.NextPlatform)
                {
                    m_mapView.ToNextPlatform();

                    var newHeight = m_mapView.CurrentPlatform.Height;
                    m_redZoneTargetHeight += (newHeight - lastHeight) * redZoneSpeed;

                    if (m_lastScoreHeight < newHeight)
                    {
                        m_lastScoreHeight = newHeight;
                        Score++;
                        m_gameScreen.OnGameScoreChenged(Score);
                    }
                }
            }
            if (m_isLongFall)
                LongFallEnd();
        }

        private void LongFallBegin()
        {
            if (m_mapView.CurrentPlatform.Type != SJ.Model.PlatformType.SaveRing)
            {
                m_mapView.ToSavePlatform();
                m_gameScreen.OnLongFallBegin();
                m_isLongFall = true;
                m_updatePlatforms = false;
            }
        }

        private void LongFallEnd()
        {
            m_gameScreen.OnLongFallEnd();
            m_isLongFall = false;
            m_updatePlatforms = true;
        }

        private void StartGame()
        {
            m_player.OnGameStart();
        }

        private void GameOver(bool success, SJ.View.PlatformEffector effector, int sector = -1)
        {
            m_gameOver = true;

            int lastBest = PlayerPrefs.GetInt("Best", 0);
            if (lastBest < Score)
                PlayerPrefs.SetInt("Best", Score);

            //int lastCoins = PlayerPrefs.GetInt("Coins", 0);
            //PlayerPrefs.SetInt("Coins", lastCoins + m_lastLevel);

            if (effector != null)
            {
                effector.SetHilight(sector, true);
                m_hilightedEffector = effector;
                m_hilightedSector = sector;
            }

            m_gameScreen.OnGameOver();
            m_player.OnGameOver(success);
        }

        public void Continue()
        {
            var current = m_mapView.CurrentPlatform;
            var next = m_mapView.NextPlatform;

            if (current != null && current.Type != SJ.Model.PlatformType.SaveRing) current.Visible = false;
            if (next != null && next.Type != SJ.Model.PlatformType.SaveRing) next.Visible = false;

            m_updatePlatforms = false;

            if (m_hilightedEffector != null)
            {
                m_hilightedEffector.SetHilight(m_hilightedSector, false);
                m_hilightedEffector = null;
                LongFallBegin();
            }

            float targetHeight = m_mapView.CurrentPlatform.Height - m_startRedZoneDistance;
            if (m_redZoneHeight > targetHeight)
            {
                m_redZoneHeight = targetHeight;
                m_redZoneTargetHeight = m_redZoneHeight;
                m_mapView.SetRedHeight(m_redZoneHeight);
            }

            // Destroy(m_gameScreen.gameObject);
            InitGameScreen();
            m_gameScreen.OnGameContinue();
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