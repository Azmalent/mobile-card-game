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
        //[SerializeField] private HingeJoint m_hingeJoint = null;
        [SerializeField] private GameObject m_spiralCenter = null;
        //[SerializeField] private AmorphusObject m_amorphusObject = null;

        [SerializeField] private GameObject m_playerContactBlockPS = null;
        [SerializeField] private GameObject m_playerContactDashPS = null;
        //[SerializeField] private GameObject m_playerContactSavePS = null;
        //[SerializeField] private GameObject m_playerContactEndPS = null;
        [SerializeField] private GameObject m_testPS = null;

        private GameObject m_testPSObj = null;

        public Vector3 testPSOffset;

        [Space]
        [Range(6, 60)] public int solverIterations = 6;
        public bool updateSolverIterations = false;

        [Space]
        public float rotateSense = 1f;

        [Space]
        [SerializeField] private float m_speed;
        public float targetSpeed = 8f;
        public float speedZoneOfComfort = 1f;
        public float speedRecoveryPower = 8f;
        public float speedAfterBlock = 1f;
        public float speedAfterDash = 12f;

        [Space]
        public float fallVelocity = 1;
        public float baseGravity;
        public float gravity;
        public float jumpHeight = 1;
        public bool overrideGravity = false;
        public float currentGravity;
        public int rushPlatformCount = 3;

        [Space]
        public float afterGameCameraSpeed = 10;
        public float afterGameCameraOffset = 3;

        [Space]
        public Vector3 overridedDirection;
        public bool overrideDirection = false;

        [Space]
        public Vector3 currentDirection;
        public Vector3 currentVelocity;

        [Space]
        public float maxSpeed;
        public float maxYVelosity;

        [Space]
        public JumpStat jumpStat = new JumpStat();

        public Vector3 Position => m_rigidbody.transform.position;
        public Vector3 Scale => m_rigidbody.transform.localScale;

        public bool IsJump { get; private set; } = true;
        public bool IsFall => !IsJump;

        public float Size { get; private set; }

        //private View.IMap m_mapView = null;
        private int m_playerActionIndex = 0;

        private VirtualStick m_virtualStick;

        private bool m_active = false;
        private float m_stickMoveDelta = 0;

        private LinkedList<View.DashParticleHelper> m_dashParticles = new LinkedList<View.DashParticleHelper>();


        public void Init(SpiralJumper.View.IMap mapView) {
            //m_mapView = mapView;
            //mapView.ToNextPlatform();
        }

        private void Awake() {
            if (!m_rigidbody || !m_spiralCenter || !m_player || !m_body
                || !m_playerContactBlockPS || !m_camera || !m_playerContactDashPS)
                Debug.LogError("Not all set in " + GetType());

            DiGro.Check.CheckComponent<SphereCollider>(m_rigidbody.gameObject);
            DiGro.Check.CheckComponent<View.DashParticleHelper>(m_playerContactDashPS);
            
            Size = m_rigidbody.GetComponent<SphereCollider>().radius * 2;

            m_player.PlatformEnterEvent += OnPlatformEnter;
            baseGravity = -Physics.gravity.y;
            currentGravity = -Physics.gravity.y;

            maxYVelosity = Mathf.Sqrt(2 * currentGravity * jumpHeight);

            DiGro.Check.CheckComponent<VirtualStick>(gameObject);
            m_virtualStick = gameObject.GetComponent<VirtualStick>();

            m_rigidbody.isKinematic = true;
        }

        private void Start() {
            var ps = Instantiate(m_testPS).GetComponent<ParticleSystem>();
            ps.transform.parent = transform.parent;
            ps.transform.position = m_spiralCenter.transform.position + testPSOffset;
            ps.Play();
            m_testPSObj = ps.gameObject;
        }

        private void OnDestroy() { }

        private void Update() {
            if (!m_active)
                return;

            if (updateSolverIterations) {
                updateSolverIterations = false;
                m_rigidbody.solverIterations = solverIterations;
            }

            if (m_virtualStick.HandleInput())
                m_stickMoveDelta += m_virtualStick.MoveDelta;
        }

        private void FixedUpdate() {
            if (m_active) {
                UpdateSpeed();

                var angle = m_rigidbody.transform.rotation.eulerAngles.y;
                var position = m_rigidbody.position;
                position.y += m_speed * Time.fixedDeltaTime;

                m_rigidbody.MovePosition(position);

                if (m_stickMoveDelta != 0) {
                    angle = Rotate(m_stickMoveDelta);
                    m_stickMoveDelta = 0;
                }
            }

            m_testPSObj.transform.position = m_spiralCenter.transform.position + testPSOffset;
        }

        private void UpdateSpeed() {
            float speedDist = m_speed - targetSpeed;
            float sigma = speedZoneOfComfort / 2;
            float recoveryWish = 1 - Mathf.Exp(-0.5f * Mathf.Pow(speedDist / sigma, 2));

            float recovery = recoveryWish * speedRecoveryPower * Time.fixedDeltaTime;

            m_speed += recovery * -Mathf.Sign(speedDist);
        }

        public void OnGameStart() {
            m_active = true;
        }

        public void OnGameOver(bool success) {
            m_active = false;
            AudioManager.StartSound(SoundType.GameOver);
            Vibrate.Fail();
        }

        public void OnGameContinue() {
            m_active = true;
        }

        private void OnPlatformEnter(SJ.View.PlatformEffector effector, int sector, bool centerOnEffector) {

            PlatformEnterListener?.Invoke(effector, sector);

            var effect = effector.GetEffect(sector);

            if(effect == SJ.Model.PlatformEffect.None) {
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
                                
                for (var node = m_dashParticles.First; node != null; ) {
                    var next = node.Next;
                    node.Value.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    node = next;
                } 
            }
            if(effect == SJ.Model.PlatformEffect.Red) {
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

    }
}