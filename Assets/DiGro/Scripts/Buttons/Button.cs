using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

using DiGro.Input;
using DiGro.Audio;

namespace DiGro {

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(InputHandler))]
    public class Button : MonoBehaviour {

        public enum State { Idle, Touch, Cancel, Release, Enter, Exit }
        public enum SwipeAction { Break, Continue }

        [Header("Dots.Button")]

        [SerializeField] protected GameObject m_raycastObject = null;

        [Header("Animations triggers")]
        [SerializeField] protected string m_idleTrigger = "Idle";
        [SerializeField] protected string m_touchTrigger = "Touch";
        [SerializeField] protected string m_cancelTrigger = "Cancel";
        [SerializeField] protected string m_releaseTrigger = "Release";
        [SerializeField] protected string m_enterTrigger = "Enter";
        [SerializeField] protected string m_exitTrigger = "Exit";

        [Header("Input")]
        [SerializeField] private int m_inputLayer = 0;
        [SerializeField] private SwipeAction m_onSwipeStartAction = SwipeAction.Continue;

        [Header("Sound")]
        public SoundType sound = SoundType.None;

        [Header("Play")]
        public bool playEnter = false;
        public bool playExit = false;
        public bool playIdle = false;

        protected Animator m_animator = null;
        protected Rect? m_viewportRect = null;

        protected InputHandler m_handler = null;


        public Action OnClickEvent;
        public Action OnBeginTouch;
        public Action OnEndTouch;

        public bool Touched { get; protected set; }
        public bool TouchLocked { get; protected set; }
        public bool CanTouch { get { return !TouchLocked; } }
        public Rect ViewportRect { set { m_viewportRect = value; } }

        protected virtual void Awake() {
            if (!m_raycastObject)
                Debug.LogError("Not all set in " + GetType());

            m_animator = GetComponent<Animator>();
            m_handler = GetComponent<InputHandler>();
        }

        private void Start() {
            m_handler.PointerDownListener += OnPointerDown;
            m_handler.PointerUpListener += OnPointerUp;
            m_handler.DragListener += OnDrag;
        }

        protected virtual void OnDestroy() {
            m_handler.PointerDownListener -= OnPointerDown;
            m_handler.PointerUpListener -= OnPointerUp;
            m_handler.DragListener -= OnDrag;
        }

        protected virtual void Update() {
            if (playEnter) {
                playEnter = false;
                m_animator.SetTrigger(m_enterTrigger);
            }
            if (playExit) {
                playExit = false;
                m_animator.SetTrigger(m_exitTrigger);
            }
            if (playIdle) {
                playIdle = false;
                m_animator.SetTrigger(m_idleTrigger);
            }
        }

        public virtual void Enter() { m_animator.SetTrigger(m_enterTrigger); }
        public virtual void Exit() { m_animator.SetTrigger(m_exitTrigger); }

        protected virtual void Touch() {
            Touched = true;
            m_animator.SetTrigger(m_touchTrigger);
            OnBeginTouch?.Invoke();
        }

        protected virtual void CancelTouch() {
            Touched = false;
            m_animator.SetTrigger(m_cancelTrigger);
            OnEndTouch?.Invoke();
        }

        protected virtual void ReleaseTouch() {
            Touched = false;
            m_animator.SetTrigger(m_releaseTrigger);
            if (sound != SoundType.None)
                AudioManager.StartSound(sound);
            OnClickEvent?.Invoke();
            OnEndTouch?.Invoke();
        }

        protected virtual void OnPointerDown(EventData ev) {
            if (!gameObject.activeSelf || !enabled)
                return;
            if (m_viewportRect != null && !m_viewportRect.Value.Contains(ev.eventData.position))
                return;
            var currentRaycasted = ev.eventData.pointerCurrentRaycast.gameObject;
            if (CanTouch && m_raycastObject == currentRaycasted)
                Touch();
        }

        protected virtual void OnPointerUp(EventData ev) {
            if (!gameObject.activeSelf || !enabled)
                return;
            if (m_viewportRect != null && !m_viewportRect.Value.Contains(ev.eventData.position))
                return;

            var currentRaycasted = ev.eventData.pointerCurrentRaycast.gameObject;
            if (Touched) {
                if (m_onSwipeStartAction == SwipeAction.Break && ev.pressWorldPosition != ev.worldPosition)
                    CancelTouch();
                else if (m_raycastObject == currentRaycasted)
                    ReleaseTouch();
                else
                    CancelTouch();
            }
        }

        protected virtual void OnDrag(EventData ev) {
            if (gameObject.activeSelf && enabled) {
                var currentRaycasted = ev.eventData.pointerCurrentRaycast.gameObject;
                if (m_viewportRect != null && !m_viewportRect.Value.Contains(ev.eventData.position))
                    return;

                if (Touched) {
                    if (m_onSwipeStartAction == SwipeAction.Break)
                        CancelTouch();
                    else if (m_raycastObject != currentRaycasted)
                        CancelTouch();
                }
            }
        }

    }
}
