using System;
using System.Collections.Generic;

using UnityEngine;


namespace DiGro.ScreenSystem {

    public class Screen : MonoBehaviour {
        [SerializeField] protected Canvas m_canvas = null;
        [SerializeField] protected Animator m_animator = null;

        [SerializeField] private AnimationList m_enterAnimationList = null;
        [SerializeField] private AnimationList m_exitAnimationList = null;
        private List<CachedDelayedAnimation> m_cachedEnterAnimations = new List<CachedDelayedAnimation>();
        private List<CachedDelayedAnimation> m_cachedExitAnimations = new List<CachedDelayedAnimation>();

        private bool m_animationListsCashed = false;

        //protected Action OnEndPlayAction;
        public Action OnEnterAction;
        public Action OnExitAction;


        protected virtual void Awake() {
            if (!m_canvas || !m_animator)
                Debug.LogError("Not all set in " + GetType());
            m_canvas.worldCamera = Camera.main;

            if (!m_animationListsCashed) {
                if (m_enterAnimationList != null)
                    CacheAnimationList(m_enterAnimationList, m_cachedEnterAnimations);
                if (m_exitAnimationList != null)
                    CacheAnimationList(m_exitAnimationList, m_cachedExitAnimations);
                m_animationListsCashed = true;
            }
        }

        protected virtual void Start() { }
        protected virtual void OnDestroy() { }


        private void CacheAnimationList(AnimationList animationList, List<CachedDelayedAnimation> cachedlist) {
            for (int i = 0; i < animationList.Count; i++) {
                var delayed = animationList[i];
                bool success = false;
                var obj = transform.Find(delayed.animator);
                if (obj != null) {
                    var animator = obj.GetComponent<Animator>();
                    if (animator != null) {
                        var cache = new CachedDelayedAnimation();
                        cache.animator = animator;
                        cache.animation = delayed;
                        cachedlist.Add(cache);
                        success = true;
                    }
                }
                if (!success)
                    Debug.LogError(GetType() + ": Animation target object \"" + delayed.animator + "\" not found and not be cached.");
            }
        }

        private void StartDelayedAnimations(List<CachedDelayedAnimation> cachedlist) {
            for (int i = 0; i < cachedlist.Count; i++) {
                var delayed = cachedlist[i];
                if (delayed.animation.delay == 0)
                    delayed.Invoke();
                else
                    this.StartDeleyed(delayed.Invoke, delayed.animation.delay);
            }
        }

        public virtual void Enter(Context context) {
            //OnEnterAction = onEnterAction;
            //m_animator.SetTrigger("Enter");
            m_animator.Play("Enter");
            StartDelayedAnimations(m_cachedEnterAnimations);
        }

        public virtual void Exit() {
            //OnExitAction = onExitAction;
            //m_animator.SetTrigger("Exit");
            m_animator.Play("Exit");
            StartDelayedAnimations(m_cachedExitAnimations);
        }

        //protected virtual void OnEndPlay() {
        //    var action = OnEndPlayAction;
        //    OnEndPlayAction = null;
        //    action?.Invoke();
        //}

        #region AnimationEvents

        protected virtual void OnExit() {
            OnExitAction?.Invoke();
            //OnEndPlay();
        }

        protected virtual void OnEnter() {
            OnEnterAction?.Invoke();
            //OnEndPlay();
        }

        public virtual void OnDialogExit(DialogResult result) { }

        #endregion
    }

}
