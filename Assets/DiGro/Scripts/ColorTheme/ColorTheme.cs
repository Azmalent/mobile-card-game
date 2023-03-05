using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiGro {

    public class ColorTheme : SingletonMono<ColorTheme> {

        public event Action OnPalleteChange;

        [SerializeField] private Camera m_camera;
        [Space]
        [SerializeField] private List<Pallete> m_palletes = new List<Pallete>();


        public Pallete i_testPallete;
        public bool i_useThis = false;
        public bool i_update = false;

        private int m_currentPalleteIndex = 0;

        private Pallete CurrentPallete { get { return get.m_palletes[get.m_currentPalleteIndex]; } }


        private void Awake() {
            if (!m_camera || m_palletes.Count == 0)
                Debug.LogError("Not all set in " + GetType());
            m_palletes = new List<Pallete>(Resources.LoadAll<Pallete>("Palletes"));
        }

        private void Start() {
            ReloadTheme();
        }

        private void Update() {
            if (i_update) {
                i_update = false;
                UpdateColors();
            }
            if (i_useThis && i_testPallete != null) {
                if (i_testPallete.update) {
                    i_testPallete.update = false;
                    UpdateColors();
                }
            }
        }


        public static void ReloadTheme() {
            //string palleteId = PlayerPrefs.GetString(Constants.PlayerPrefs.KeySelectedTheme);
            //get.m_currentPalleteIndex = get.m_palletes.FindIndex(delegate (Pallete pallete) { return pallete.id == palleteId; });
            //if (get.m_currentPalleteIndex < 0 || get.m_currentPalleteIndex >= get.m_palletes.Count)
            //    throw new IndexOutOfRangeException();
            UpdateColors();
        }

        public static void UpdateColors() {
            var color = get.CurrentPallete.GetColor(PalletColor.Camera);
            if (get.i_useThis && get.i_testPallete != null)
                color = get.i_testPallete.GetColor(PalletColor.Camera);

            get.m_camera.backgroundColor = color;
            if (get.OnPalleteChange != null)
                get.OnPalleteChange.Invoke();
        }

        public static Color GetColor(PalletColor type) {
            if (get.i_useThis && get.i_testPallete != null)
                return get.i_testPallete.GetColor(type);
            return get.CurrentPallete.GetColor(type);
        }
    }

}