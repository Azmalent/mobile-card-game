using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SpiralJumper.Audio;
using SpiralJumper.Amorphus;
using SpiralJumper.Controller;

namespace SpiralRunner.Controller {

    [AddComponentMenu("Player Controller (SR)")]
    public class PlayerController : MonoBehaviour, IPlayerController {
        public event Action<SpiralJumper.View.PlatformEffector, int> PlatformEnterListener;

        [SerializeField] private SpiralPlatformer.SpiralFolowedCamera m_camera = null;
        [SerializeField] private SpiralJumper.View.Player m_player = null;
        [SerializeField] private Rigidbody m_rigidbody = null;
        //[SerializeField] private HingeJoint m_hingeJoint = null;
        [SerializeField] private GameObject m_spiralCenter = null;
        //[SerializeField] private AmorphusObject m_amorphusObject = null;

        [SerializeField] private GameObject m_playerContactNextPS = null;
        [SerializeField] private GameObject m_playerContactSavePS = null;
        [SerializeField] private GameObject m_playerContactEndPS = null;
        [SerializeField] private GameObject m_testPS = null;

        private GameObject m_testPSObj = null;

        public Vector3 testPSOffset;

        [Space]
        [Range(6, 60)] public int solverIterations = 6;
        public bool updateSolverIterations = false;
        [Space]
        public float rotateSense = 1f;
        public float speed = 1f;
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


        public void Init(SpiralJumper.View.IMap mapView) {
            //m_mapView = mapView;
            mapView.ToNextPlatform();
        }


        private void Awake() {
            if (!m_rigidbody || !m_spiralCenter || !m_player
                || !m_playerContactNextPS || !m_playerContactSavePS || !m_playerContactEndPS
                || !m_camera)
                Debug.LogError("Not all set in " + GetType());

            DiGro.Check.CheckComponent<SphereCollider>(m_rigidbody.gameObject);
            
            Size = m_rigidbody.GetComponent<SphereCollider>().radius * 2;

            //m_player.PlatformEnterListener += OnPlatformEnter;
            baseGravity = -Physics.gravity.y;
            currentGravity = -Physics.gravity.y;

            maxYVelosity = Mathf.Sqrt(2 * currentGravity * jumpHeight);

            DiGro.Check.CheckComponent<VirtualStick>(gameObject);
            m_virtualStick = gameObject.GetComponent<VirtualStick>();
            //m_virtualStick.StickMoveEvent += OnStickMove;

            //Physics.gravity = Vector3.up * 0.3f;// -Physics.gravity;

            m_rigidbody.isKinematic = true;
        }

        private void Start() {
            var ps = Instantiate(m_testPS).GetComponent<ParticleSystem>();
            ps.transform.parent = transform.parent;
            ps.transform.position = m_spiralCenter.transform.position + testPSOffset;
            ps.Play();
            m_testPSObj = ps.gameObject;

            //PlayerCommands.get.OnCommand += PlayerCommandListener;

            //if (SpiralJumper.get.playerActionsCommand == SpiralJumper.PlayerActionsCommand.Use)
            //    StartCoroutine(PlayActions(SpiralJumper.get.playerActions));
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
                var pos = m_rigidbody.position;
                pos.y += speed * Time.fixedDeltaTime;
                m_rigidbody.MovePosition(pos);

                if (m_stickMoveDelta != 0) {
                    Rotate(m_stickMoveDelta);
                    m_stickMoveDelta = 0;
                }
            }

            m_testPSObj.transform.position = m_spiralCenter.transform.position + testPSOffset;
        }

        public void OnGameStart() {
            //m_rigidbody.isKinematic = true;
            m_active = true;
        }

        public void OnGameOver(bool success) {
            m_active = false;
            //m_rigidbody.isKinematic = true;
            AudioManager.StartSound(SoundType.GameOver);
            Vibrate.Fail();
        }

        public void OnGameContinue() {
            //m_rigidbody.isKinematic = true;
            m_active = true;
        }

        //private void OnPlatformEnter(View.PlatformEffector effector, int sector, bool centerOnEffector) {
        //    var lastCurrent = m_mapView.CurrentPlatform;
        //    var lastNext = m_mapView.NextPlatform;
        //    var effect = effector.GetEffect(sector);

        //    PlatformEnterListener?.Invoke(effector, sector);

        //    if (!centerOnEffector) {
        //        SwipeFrom(effector);
        //    }
        //    else if (IsFall) {
        //        if (effect == Model.PlatformEffect.Jump)
        //            test_Jump3();
        //        else
        //            Jump();

        //        if (m_mapView.CurrentPlatform == lastNext) {
        //            if (lastNext.Type == Model.PlatformType.SaveRing) {
        //                var ps = Instantiate(m_playerContactSavePS).GetComponent<ParticleSystem>();
        //                ps.transform.parent = transform;
        //                ps.transform.position = lastNext.transform.position;
        //                var main = ps.main;
        //                var color = SpiralJumper.get.savePlatformColor;
        //                main.startColor = new ParticleSystem.MinMaxGradient(color, color);
        //                ps.Play();
        //            }
        //            else {
        //                var ps = Instantiate(m_playerContactNextPS).GetComponent<ParticleSystem>();
        //                ps.transform.parent = transform;
        //                ps.transform.position = m_player.transform.position;
        //                var main = ps.main;
        //                var color = SpiralJumper.get.activePlatformColor;
        //                main.startColor = new ParticleSystem.MinMaxGradient(color, color);
        //                ps.Play();
        //            }
        //            if (effect != Model.PlatformEffect.Red) {
        //                AudioManager.StartSound(SoundType.Jump);
        //                Vibrate.Fine();
        //            }
        //        }
        //        else {
        //            AudioManager.StartSound(SoundType.Jump);
        //            Vibrate.Fine();
        //        }
        //    }
        //    else {
        //        if (effector.Platform == m_mapView.CurrentPlatform) {
        //            Jump();
        //            AudioManager.StartSound(SoundType.Jump);
        //        }
        //        else if (effect == Model.PlatformEffect.Red) {
        //            SwipeFrom(effector);
        //        }
        //    }
        //}

        //private void Jump() {
        //    var platform = m_mapView.NextPlatform;
        //    if (platform != null) {
        //        float angle = AngleBetweenPlayerAndPlatform(platform);
        //        var xOnP = ProjectionAngleOnPerpendicular(angle);

        //        var jumpDirectin = new Vector3(xOnP.x, 1, xOnP.y);
        //        if (overrideDirection)
        //            jumpDirectin = overridedDirection;

        //        currentDirection = jumpDirectin;
        //        currentDirection.y = 1;
        //        currentVelocity = jumpDirectin * maxYVelosity;
        //        m_rigidbody.velocity = currentVelocity;

        //        IsJump = true;

        //        if (jumpStat != null) {
        //            var playerPos = m_rigidbody.transform.position;
        //            jumpStat.StopStep(playerPos);
        //            jumpStat.StartStep(playerPos, angle);
        //        }
        //    }
        //}

        private float Rotate(float rotateDelta) {
            float angle = rotateDelta * rotateSense;

            //var xOnP = ProjectionAngleOnPerpendicular(angle);

            //var rotateDirectin = new Vector3(xOnP.x, 0, xOnP.y);

            //currentDirection = rotateDirectin;
            ////currentVelocity = rotateDirectin * maxYVelosity;
            //currentVelocity = rotateDirectin;

            //var vel = m_rigidbody.velocity;
            //vel.x = currentVelocity.x;
            //vel.z = currentVelocity.z;

            //m_rigidbody.velocity = vel;

            float targetAngle = m_rigidbody.transform.rotation.eulerAngles.y - angle;

            m_rigidbody.MoveRotation(Quaternion.AngleAxis(targetAngle, Vector3.up));

            return targetAngle;
        }

        //private void Fall() {
        //    m_rigidbody.velocity = Vector3.down * fallVelocity;
        //    IsJump = false;

        //    //var command = new PlayerCommands.Command();
        //    //command.action = PlayerCommands.Action.Fall;
        //    //command.vec1 = m_rigidbody.position;
        //    //command.vec2 = m_rigidbody.velocity;

        //    //PlayerCommands.get.Add(command);
        //}

        //private void Fall(PlayerCommands.Command command) {
        //    m_rigidbody.position = command.vec1;
        //    m_rigidbody.velocity = command.vec2;
        //    IsJump = false;
        //}

        //private void SwipeFrom(SpiralJumper.View.PlatformEffector effector) {
        //    var platform = effector.Platform;
        //    float compare = CompareWithPlatform(m_rigidbody.transform.position, platform);

        //    float angle = compare > 0 ? platform.Length : -platform.Length;
        //    var xOnP = ProjectionAngleOnPerpendicular(angle);
        //    var swipeDirectin = new Vector3(xOnP.x, 0, xOnP.y);

        //    m_rigidbody.velocity = swipeDirectin * maxYVelosity;
        //}

        //private float AngleBetweenPlayerAndPlatform(SpiralJumper.View.PlatformBase platform) {
        //    var playerPos = m_rigidbody.transform.position;
        //    var centerPos = m_spiralCenter.transform.position;
        //    var radius = (playerPos - centerPos).magnitude;
        //    var platformPos = platform.transform.position;
        //    var rot = platform.transform.rotation;
        //    var euler = rot.eulerAngles;

        //    var point3 = Quaternion.Euler(euler.x, euler.y + platform.Length / 2, euler.z) * Vector3.forward * radius;
        //    point3.y = platformPos.y;

        //    var platformDir3 = point3 - centerPos;
        //    var platformDir2 = new Vector2(platformDir3.x, platformDir3.z);
        //    var playerDir3 = playerPos - centerPos;
        //    var playerDir2 = new Vector2(playerDir3.x, playerDir3.z);

        //    var angle = Vector2.SignedAngle(playerDir2, platformDir2);
        //    return angle;
        //}

        /// <summary>
        /// Сравнивает положение точки относительно средней линии плптформы.
        /// </summary>
        /// <returns>
        /// Если меньше нуля, точка лежит слева.
        /// Если больше нуля, точка лежит справа.
        /// Если равна нулу, точка лежит на линии.
        /// </returns>
        //private float CompareWithPlatform(Vector3 point, SpiralJumper.View.PlatformBase platform) {
        //    var centerPos = m_rootRigidbody.transform.position;
        //    var rot = platform.transform.rotation;
        //    var euler = rot.eulerAngles;

        //    var point3 = Quaternion.Euler(euler.x, euler.y + platform.Length / 2, euler.z) * Vector3.forward;

        //    var p0 = new Vector2(point.x, point.z);
        //    var p1 = new Vector2(centerPos.x, centerPos.z);
        //    var p2 = new Vector2(point3.x, point3.z);

        //    return DiGro.LineMath.PseudoDotProduct(p0, p1, p2);
        //}

        //private Vector2 ProjectionAngleOnPerpendicular(float angle) {
        //    var n = (m_spiralCenter.transform.position - m_rigidbody.transform.position).normalized;
        //    var p = -Vector2.Perpendicular(new Vector2(n.x, n.z));
        //    return p * angle / 180;
        //}

        //private void PlayerCommandListener(PlayerCommands.Command command) {
        //    if (command.action == PlayerCommands.Action.Fall)
        //        Fall(command);
        //}



    }
}