//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//using DanielLochner.Assets.SimpleScrollSnap;

//namespace SpiralJumper
//{
//    public class CharactersScreen : MonoBehaviour
//    {
//        //public DiGro.UButton cancelButton;
//        //public DiGro.UButton revardButton;

//        //private Action m_onCancelAction = null;
//        [SerializeField] private View.Storage m_storage;
//        [SerializeField] private SimpleScrollSnap m_scrollSnap;
//        [SerializeField] private DiGro.UButton m_randomButton;

//        private List<View.Character> m_characters = new List<View.Character>();


//        private void Awake()
//        {
//            DiGro.Check.NotNull(m_storage);
//            DiGro.Check.NotNull(m_scrollSnap);
//            DiGro.Check.NotNull(m_randomButton);

//            m_randomButton.OnClick.AddListener(OnRandomButtonClick);
//            m_scrollSnap.onPanelChanged.AddListener(OnPanelChanged);

//            for(int i = 0; i < Characters.Count; ++i)
//            {
//                m_scrollSnap.AddToBack(Characters.GetUIPrefab());

//                var character = m_scrollSnap.Panels[i].GetComponent<View.Character>();
//                m_characters.Add(character);

//                character.Init(Characters.Desc(i));

//                int index = i;
//                character.button.onClick.AddListener(() => OnCharacterClick(index));
//            }

//            m_storage.Value = 3000;
//        }
         
//        private void OnRandomButtonClick()
//        {
//            int index = UnityEngine.Random.Range(0, m_scrollSnap.NumberOfPanels);
//            m_scrollSnap.GoToPanel(index);
//        }

//        private void OnPanelChanged()
//        {
//            int index = m_scrollSnap.CurrentPanel;
//        }

//        private void OnCharacterClick(int index)
//        {
//            if (m_scrollSnap.CurrentPanel != index)
//                return;

//            var desc = Characters.Desc(index);
//            int value = PlayerPrefs.GetInt(desc.VolumeTag, 0);
//            int target = Mathf.Clamp(value + UnityEngine.Random.Range(50, 400), 0, 1000);

//            m_storage.ChangeValue(m_storage.Value - (target - value), 1f);
//            m_characters[index].ChangeValue(value, target, 1000, 1.0f);

//            PlayerPrefs.SetInt(desc.VolumeTag, target);
//        }
//    }
//}