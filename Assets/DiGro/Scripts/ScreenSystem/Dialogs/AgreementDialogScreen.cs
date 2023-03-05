using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DiGro.ScreenSystem {

    public class AgreementDialogContext : DialogContext {
        public string contentText = "";
        public string buttonText = "";
    }

    public class AgreementDialogResult : DialogResult {
        public bool success = false;
    }

    public class AgreementDialogScreen : DialogScreen {
        [Header(nameof(AgreementDialogScreen))]

        [SerializeField] private Text m_contentText = null;
        [SerializeField] private Text m_buttonText = null;
        [SerializeField] private DiGro.Button m_actionButton = null;

        private AgreementDialogResult m_result = null;


        protected override void Awake() {
            base.Awake();
            if (!m_contentText || !m_buttonText || !m_actionButton)
                Debug.LogError("Not all set in " + GetType());
        }


        public override void Enter(Context context) {
            base.Enter(context);
            var ctx = (AgreementDialogContext)m_context;
            m_contentText.text = ctx.contentText;
            m_buttonText.text = ctx.buttonText;
            m_actionButton.OnClickEvent = OnActionButtonClick;
        }
        
        public override DialogResult GetResult() {
            if (m_result == null)
                m_result = new AgreementDialogResult { id = m_context.id };
            return m_result;
        }

        private void OnActionButtonClick() {
            m_result = new AgreementDialogResult {
                id = m_context.id,
                success = true
            };
            Exit();
        }

    }

}