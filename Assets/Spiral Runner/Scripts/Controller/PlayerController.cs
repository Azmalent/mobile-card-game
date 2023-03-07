using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SpiralJumper.Audio;
using SpiralJumper.Amorphus;
using SpiralJumper.Controller;

using SJ = SpiralJumper;
using DashHelperNode = System.Collections.Generic.LinkedListNode<SpiralRunner.View.DashParticleHelper>;

namespace SpiralRunner.Controller {

    [AddComponentMenu("Player Controller (SR)")]
    public class PlayerController : MonoBehaviour, IPlayerController {
        public event Action<SJ.View.PlatformEffector, int> PlatformEnterListener;

        [SerializeField] private SpiralPlatformer.SpiralFolowedCamera m_camera = null;
        [SerializeField] private View.Player m_player = null;
        [SerializeField] private Rigidbody m_rigidbody = null;
        [SerializeField] private GameObject m_body = null;
        [SerializeField] private GameObject m_spiralCenter = null;

        [SerializeField] private GameObject m_playerContactBlockPS = null;
        [SerializeField] private GameObject m_playerContactDashPS = null;
        [SerializeField] private GameObject m_testPS = null;

        private GameObject m_testPSObj = null;

        public Vector3 testPSOffset;

        [Space]
        public bool isPlayerShadow = false;
        
        [Space]
        public float rotateSense = 1f;

        [Space]
        public float comfortSpeed = 8f;
        public float speedZoneOfComfort = 1f;
        public float speedRecoveryPower = 8f;
        public float speedAfterBlock = 1f;
        public float speedAfterDash = 12f;

        public Vector3 Position => m_rigidbody.transform.position;

        public float Size { get; private set; }

        public bool IsLocalPlayer => !isPlayerShadow;

        private VirtualStick m_virtualStick;

        private bool m_active = false;
        private float m_stickMoveDelta = 0;

        private float m_speed;
        private float m_targetSpeed;

        private float m_shadowHeight;
        private float m_shadowAngle;

        private LinkedList<View.DashParticleHelper> m_dashParticles = new LinkedList<View.DashParticleHelper>();

        public void Init(SpiralJumper.View.IMap mapView) {
            //m_mapView = mapView;
            //mapView.ToNextPlatform();
        }

        private void Awake() {
            DiGro.Check.NotNull(m_rigidbody);
            DiGro.Check.NotNull(m_spiralCenter);
            DiGro.Check.NotNull(m_player);
            DiGro.Check.NotNull(m_body);
            DiGro.Check.NotNull(m_playerContactBlockPS);
            DiGro.Check.NotNull(m_playerContactDashPS);

            DiGro.Check.CheckComponent<SphereCollider>(m_rigidbody.gameObject);
            DiGro.Check.CheckComponent<View.DashParticleHelper>(m_playerContactDashPS);

            if (IsLocalPlayer) {
                DiGro.Check.NotNull(m_camera);
                DiGro.Check.CheckComponent<VirtualStick>(gameObject);
            }

            Size = m_rigidbody.GetComponent<SphereCollider>().radius * 2;

            m_rigidbody.isKinematic = true;
        }

        private void Start() {
            if (IsLocalPlayer) {
                
                m_virtualStick = gameObject.GetComponent<VirtualStick>();

                m_player.PlatformEnterEvent += OnPlatformEnter;

                var ps = Instantiate(m_testPS).GetComponent<ParticleSystem>();
                ps.transform.parent = transform.parent;
                ps.transform.position = m_spiralCenter.transform.position + testPSOffset;
                ps.Play();
                m_testPSObj = ps.gameObject;
            }
        }

        private void OnDestroy() { 
            if(IsLocalPlayer)
                m_player.PlatformEnterEvent -= OnPlatformEnter;
        }

        private void Update() {
            if (!m_active)
                return;

            if (IsLocalPlayer) {
                if (m_virtualStick.HandleInput())
                    m_stickMoveDelta += m_virtualStick.MoveDelta;
            }
        }

        private void FixedUpdate() {
            if (IsLocalPlayer) {
                UpdateSpeed();

                var position = m_rigidbody.position;
                position.y += m_speed * Time.fixedDeltaTime;

                m_rigidbody.MovePosition(position);

                if (m_active) {
                    var angle = m_rigidbody.transform.rotation.eulerAngles.y;

                    if (m_stickMoveDelta != 0) {
                        angle = Rotate(m_stickMoveDelta);
                        m_stickMoveDelta = 0;
                    }
                }

                if (m_testPSObj != null)
                    m_testPSObj.transform.position = m_spiralCenter.transform.position + testPSOffset;
            }

            if (isPlayerShadow) {
                var position = m_rigidbody.position;
                position.y = m_shadowHeight;

                m_rigidbody.MovePosition(position);
                m_rigidbody.MoveRotation(Quaternion.AngleAxis(m_shadowAngle, Vector3.up));
            }
        }

        private void UpdateSpeed() {
            float speedDist = m_speed - m_targetSpeed;
            float sigma = speedZoneOfComfort / 2;
            float recoveryWish = 1 - Mathf.Exp(-0.5f * Mathf.Pow(speedDist / sigma, 2));

            float recovery = recoveryWish * speedRecoveryPower * Time.fixedDeltaTime;

            m_speed += recovery * -Mathf.Sign(speedDist);
        }

        public void OnGameStart() {
            if (!IsLocalPlayer)
                return;

            m_active = true;

            m_targetSpeed = comfortSpeed;
            m_speed = 0;
        }

        public void OnGameOver() {
            if (!IsLocalPlayer)
                return;

            m_active = false;

            m_targetSpeed = 0;
            m_speed = comfortSpeed;

            AudioManager.StartSound(SoundType.GameOver);
            Vibrate.Fail();

            RemoveDashParticles();
        }

        public void OnGameContinue() {
            if (!IsLocalPlayer)
                return;

            m_active = true;

            m_targetSpeed = comfortSpeed;
            m_speed = 0;
        }

        private void OnPlatformEnter(SJ.View.PlatformEffector effector, int sector, bool centerOnEffector) {
            PlatformEnterListener?.Invoke(effector, sector);

            var effect = effector.GetEffect(sector);

            if (effect == SJ.Model.PlatformEffect.None) {
                m_speed = speedAfterBlock;

                var ps = Instantiate(m_playerContactBlockPS).GetComponent<ParticleSystem>();
                ps.transform.parent = transform;
                ps.transform.position = m_body.transform.position;
                var main = ps.main;
                var color = SpiralRunner.get.activePlatformColor;
                main.startColor = new ParticleSystem.MinMaxGradient(color, color);
                ps.Play();

                AudioManager.StartSound(SoundType.Jump);
                Vibrate.Fail();

                RemoveDashParticles();
            }
            if (effect == SJ.Model.PlatformEffect.Red) {
                m_speed = speedAfterDash;

                var ps = Instantiate(m_playerContactDashPS).GetComponent<ParticleSystem>();
                ps.transform.parent = transform;
                ps.transform.position = m_body.transform.position;
                ps.Play();

                var dashHelper = ps.GetComponent<View.DashParticleHelper>();
                dashHelper.node = m_dashParticles.AddLast(dashHelper);
                dashHelper.ps = ps;

                AudioManager.StartSound(SoundType.Jump);
                Vibrate.Fine();
            }
        }


        private float Rotate(float rotateDelta) {
            float angle = rotateDelta * rotateSense;

            float targetAngle = m_rigidbody.transform.rotation.eulerAngles.y - angle;

            m_rigidbody.MoveRotation(Quaternion.AngleAxis(targetAngle, Vector3.up));

            return targetAngle;
        }

        private void RemoveDashParticles() {
            for (var node = m_dashParticles.First; node != null;) {
                var next = node.Next;
                node.Value.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                node = next;
            }
        }

    }
}