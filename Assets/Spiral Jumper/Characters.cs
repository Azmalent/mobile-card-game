using System;
using System.Collections.Generic;
using UnityEngine;


namespace SpiralJumper
{
    public class Characters : DiGro.SingletonMono<Characters>
    {
        [SerializeField] private GameObject m_uiPrefab;
        [SerializeField] private List<CharacterDescriptor> m_characters = new List<CharacterDescriptor>();

        public static int Count => get.m_characters.Count;

        private void Awake()
        {
            DiGro.Check.CheckComponent<View.Character>(m_uiPrefab);
            DiGro.Check.Index(m_characters.Count, int.MaxValue, 1);

            foreach(var desc in m_characters)
            {
                DiGro.Check.NotNull(desc.prefab);
                DiGro.Check.NotNull(desc.sprite);
            }
        }

        public static CharacterDescriptor Desc(int characterId)
        {
            DiGro.Check.Index(characterId, Count);
            return get.m_characters[characterId];
        }

        public static GameObject GetUIPrefab() => get.m_uiPrefab;
    }
}