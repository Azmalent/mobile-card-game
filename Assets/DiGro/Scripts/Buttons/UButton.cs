using System;
using System.Collections.Generic;
using UnityEngine;

using UIButton = UnityEngine.UI.Button;


namespace DiGro
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    [RequireComponent(typeof(Animator))]
    public class UButton : MonoBehaviour
    {
        [SerializeField] private string m_touchState = "Touch";

        public UIButton.ButtonClickedEvent OnClick => m_onClick;

        public bool Touched => m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_touchState);


        private Animator m_animator = null;
        private UnityEngine.UI.Button m_button = null;
        private UIButton.ButtonClickedEvent m_onClick = new UIButton.ButtonClickedEvent();


        private void Awake()
        {
            m_animator = GetComponent<Animator>();

            m_button = GetComponent<UIButton>();
            m_button.onClick = m_onClick;
        }
    }
}