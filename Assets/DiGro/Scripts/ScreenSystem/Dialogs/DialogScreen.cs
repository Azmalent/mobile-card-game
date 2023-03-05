using UnityEngine;
using System.Collections;

namespace DiGro.ScreenSystem {

    public class DialogScreen : Screen {
        [Header(nameof(DialogScreen))]

        [SerializeField] private BlockingSplash m_splash;

        protected Context m_context;

        public int SortingOrder {
            get { return m_splash.SortingOrder; }
            set {
                m_splash.SortingOrder = value;
                m_canvas.sortingOrder = value + 1;
            }
        }


        protected override void Awake() {
            base.Awake();
            if (!m_splash)
                Debug.LogError("Not all set in " + GetType());
            m_splash.OnClickAction += OnSplashClick;
        }


        public override void Enter(Context context) {
            base.Enter(context);
            m_context = context;
        }

        public virtual DialogResult GetResult() {
                return new DialogResult { id = m_context.id };
        }

        /// <summary>
        /// Возврат к диалогу при закрытии диалога выше.
        /// </summary>
        public virtual void OnFocuseEnter(DialogResult result) { }

        /// <summary>
        /// Выход из диалога при открытии диалога выше.
        /// </summary>
        public virtual void OnFocuseExit() { }

        private void OnSplashClick() {
            Debug.Log("OnSplashClick");
            CloseDialogScreen();
        }

        protected virtual void CloseDialogScreen() { Exit(); }



    }
}