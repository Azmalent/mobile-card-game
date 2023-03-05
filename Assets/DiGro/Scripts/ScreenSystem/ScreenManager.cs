using System;
using System.Collections.Generic;

using UnityEngine;


namespace DiGro.ScreenSystem {

    public class ScreenManager : SingletonMono<ScreenManager> {
        
        [SerializeField] private GameObject m_logoScreenPrefab = null;
        [SerializeField] private GameObject m_gameScreenPrefab = null;
        [SerializeField] private GameObject m_mapEditorScreenPrefab = null;

        private Screen m_currentScreen = null;
        private GameObject m_nextScreenPrefab = null;
        private ScreenContext m_nextScreenContext = null;

        private Stack<DialogScreen> m_dialogStack = new Stack<DialogScreen>();


        private void Awake() {
            if (!m_gameScreenPrefab || !m_logoScreenPrefab || !m_mapEditorScreenPrefab)
                Debug.LogError("Not all set in " + GetType());
        }

        private void Start() {
            m_currentScreen = Instantiate(m_logoScreenPrefab).GetComponent<Screen>();
            m_currentScreen.OnEnterAction = OnLogoEnter;
            m_currentScreen.OnExitAction = OnScreenExit;
            m_currentScreen.Enter(CreateContext());
        }
               
        private ScreenContext CreateContext() {
            return new ScreenContext();
        }
        
        public static void TransitToGameScreen() {
            get.m_nextScreenPrefab = get.m_gameScreenPrefab;
            get.m_nextScreenContext = get.CreateContext();
            get.m_currentScreen.Exit();
        }
        
        public void TransitToMapEditorScreen() {
            m_nextScreenPrefab = m_mapEditorScreenPrefab;
            m_nextScreenContext = CreateContext();
            m_currentScreen.Exit();
        }

        private void OnScreenEnter() { }
        private void OnScreenExit() {
            m_currentScreen.OnEnterAction = null;
            m_currentScreen.OnExitAction = null;
            Destroy(m_currentScreen.gameObject);

            m_currentScreen = Instantiate(m_nextScreenPrefab).GetComponent<Screen>();
            m_currentScreen.OnEnterAction = OnScreenEnter;
            m_currentScreen.OnExitAction = OnScreenExit;

            m_currentScreen.Enter(m_nextScreenContext);

            m_nextScreenContext = null;
            m_nextScreenPrefab = null;
        }

        private void OnLogoEnter() {
            OnScreenEnter();
            TransitToGameScreen();
        }

        public void StartDialog(GameObject dialogPrefab, DialogContext context) {
            var dialog = Instantiate(dialogPrefab).GetComponent<DialogScreen>();
            if (dialog == null)
                throw new Exception("ScreenManager: dialogPrefab havn't DialogScreen component.");

            dialog.SortingOrder = (m_dialogStack.Count + 1) * 1000;
            dialog.OnExitAction = OnDialogExit;

            if (m_dialogStack.Count > 0) {
                var currentDialog = m_dialogStack.Peek();
                currentDialog.OnFocuseExit();
            }
            m_dialogStack.Push(dialog);
            dialog.Enter(context);
        }

        private void OnDialogExit() {
            if (m_dialogStack.Count == 0)
                return;

            var currentDialog = m_dialogStack.Pop();
            var result = currentDialog.GetResult();
            Destroy(currentDialog.gameObject);

            if (m_dialogStack.Count > 0) {
                var nextDialog = m_dialogStack.Peek();
                nextDialog.OnFocuseEnter(result);
            } else if (m_currentScreen != null) {
                m_currentScreen.OnDialogExit(result);
            }

        }

    }

}