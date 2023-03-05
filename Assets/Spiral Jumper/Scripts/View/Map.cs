using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pool = DiGro.GameObjectsPool;

namespace SpiralJumper.View
{
    public class Map : MonoBehaviour, IMap
    {
        public bool endless = false;
        public int seed = 0;

        public View.PlatformBase CurrentPlatform { get; private set; }
        public View.PlatformBase NextPlatform { get; private set; }
        public int CurrentLevel { get; private set; }

       // public List<float> lastLevelDifficult = new List<float>();

        private Model.MapGenerator m_generator = null;
        private Model.DifficultGenerator m_difficultGenerator = null;
        private Model.EndlessMap m_map = null;
        private Model.Level m_lastGeneratedLevel = null;

        private List<LevelWrapper> m_levels = new List<LevelWrapper>();

        private Vector2Int m_currentPlatformIndex = -Vector2Int.one;
        private Vector2Int m_nextPlatformIndex = -Vector2Int.one;

        private GameObject m_redZoneObj = null;
        private int m_platformCount = 0;


        public void Init(Model.EndlessMap map)
        {
            m_map = map;
            m_generator = new Model.MapGenerator(m_map);
            m_difficultGenerator = new Model.DifficultGenerator(m_map.MapParams.difficultParams);
            seed = map.Seed;

            CreateMap();

            m_lastGeneratedLevel = null;
            for (int i = 0; i < 1; i++)
            {
                m_generator.GenerateLevel(ref m_lastGeneratedLevel, m_difficultGenerator.GetDifficult);
                CreateLevel(m_lastGeneratedLevel);
            }
        }

        private void Update()
        {
            if (m_currentPlatformIndex.x >= 0)
                CurrentLevel = m_levels[m_currentPlatformIndex.x].levelIndex;
        }

        private void CreateMap()
        {
            m_redZoneObj = MapFactory.get.RedZone();
            m_redZoneObj.transform.localPosition = new Vector3(0, -10, 0);
            m_redZoneObj.transform.parent = transform;
            m_redZoneObj.SetActive(false);
        }

        private void CreateLevel(Model.Level level, bool delayed = false)
        {
            var levelWrapper = new LevelWrapper();
            levelWrapper.levelIndex = level.Index;

            var pillarObj = MapFactory.get.Pillar();
            var levelLength = level.endHeight - level.beginHeight;
            pillarObj.transform.localScale = new Vector3(1, levelLength, 1);
            pillarObj.transform.localPosition = new Vector3(0, level.beginHeight + levelLength / 2, 0);
            pillarObj.transform.parent = transform;

            levelWrapper.pillar = pillarObj;
            m_levels.Add(levelWrapper);

            if (delayed)
            {
                StartCoroutine(CreateLevelCoroutine(level, levelWrapper));
                return;
            }
            for (int c = 0; c < level.chanks.Count; c++)
            {
                var chank = level.chanks[c];
                for (int p = 0; p < chank.platforms.Count; p++, m_platformCount++)
                    levelWrapper.platforms.Add(CreatePlatform(chank.platforms[p]));
            }
        }

        private IEnumerator CreateLevelCoroutine(Model.Level level, LevelWrapper levelWrapper)
        {
            for (int c = 0; c < level.chanks.Count; c++)
            {
                var chank = level.chanks[c];
                for (int p = 0; p < chank.platforms.Count; p++, m_platformCount++)
                {
                    yield return null;
                    levelWrapper.platforms.Add(CreatePlatform(chank.platforms[p]));
                }
            }
        }

        private PlatformWrapper CreatePlatform(Model.Platform platform)
        {
            var platformObj = MapFactory.get.Platform(platform.type);
            platformObj.transform.localPosition = new Vector3(0, platform.height, 0);
            platformObj.transform.localRotation = Quaternion.Euler(0, platform.angle, 0);
            platformObj.transform.parent = transform;

            var platformView = platformObj.GetComponent<View.PlatformBase>();
            platformView.Length = MapFactory.get.PlatformInfo(platform.type).length;
            platformView.Visible = false;
            platformView.gameObject.name = platform.type.ToString() + " " + m_platformCount.ToString();
            platformView.Type = platform.type;

            var platformWrapper = new PlatformWrapper();
            platformWrapper.model = platform;
            platformWrapper.view = platformView;

            return platformWrapper;
        }

        private IEnumerator DestroyLevelCoroutine(LevelWrapper levelWrapper)
        {
            MapFactory.get.PushPillar(levelWrapper.pillar);
            for(int i = 0; i < levelWrapper.platforms.Count; i++)
            {
                yield return null;
                var platformView = levelWrapper.platforms[i].view;
                MapFactory.get.PushPlatform(platformView.gameObject, platformView.Type);
            }
            levelWrapper.platforms.Clear();
        }

        public void ToNextPlatform()
        {
            if (m_levels.Count == 0)
                return;

            if (CurrentPlatform != null)
            {
                var platform = PlatformAt(m_currentPlatformIndex);
                if (platform.model.type != Model.PlatformType.SaveRing)
                    CurrentPlatform.Visible = false;
            }

            Vector2Int currentIndex;
            Vector2Int nextIndex = Vector2Int.zero;

            bool hasCurrent = TryGetNextIndex(m_currentPlatformIndex, out currentIndex);
            bool hasNext = hasCurrent ? TryGetNextIndex(currentIndex, out nextIndex) : false;

            CurrentPlatform = hasCurrent ? m_levels[currentIndex.x].platforms[currentIndex.y].view : null;
            NextPlatform = hasNext ? m_levels[nextIndex.x].platforms[nextIndex.y].view : null;

            m_currentPlatformIndex = currentIndex;
            m_nextPlatformIndex = nextIndex;

            if (CurrentPlatform != null)
            {
                CurrentPlatform.Visible = true;
                if (CurrentPlatform.Type == Model.PlatformType.SaveRing)
                    ShiftLevels();
            }
        }

        public void ToNextPlatform(int count) {
            if (m_levels.Count == 0 || count <= 0)
                return;

            if (CurrentPlatform != null) {
                var platform = PlatformAt(m_currentPlatformIndex);
                if (platform.model.type != Model.PlatformType.SaveRing)
                    CurrentPlatform.Visible = false;
            }

            Vector2Int currentIndex;
            Vector2Int nextIndex;

            bool hasCurrent = TryGetIndexFromCurrent(count, out currentIndex);
            bool hasNext = TryGetNextIndex(currentIndex, out nextIndex);

            CurrentPlatform = hasCurrent ? m_levels[currentIndex.x].platforms[currentIndex.y].view : null;
            NextPlatform = hasNext ? m_levels[nextIndex.x].platforms[nextIndex.y].view : null;

            m_currentPlatformIndex = currentIndex;
            m_nextPlatformIndex = nextIndex;

            if (CurrentPlatform != null) {
                CurrentPlatform.Visible = true;
                if (CurrentPlatform.Type == Model.PlatformType.SaveRing)
                    ShiftLevels();
            }
        }

        private PlatformWrapper PlatformAt(Vector2Int index)
        {
            return m_levels[index.x].platforms[index.y];
        }

        public void ToSavePlatform()
        {
            if (m_levels.Count == 0 && m_currentPlatformIndex.x < 0)
                return;

            if (CurrentPlatform != null)
            {
                if (CurrentPlatform.Type != Model.PlatformType.SaveRing)
                    CurrentPlatform.Visible = false;
            }
            if (NextPlatform != null)
            {
                if (NextPlatform.Type != Model.PlatformType.SaveRing)
                    NextPlatform.Visible = false;
            }
            var current = m_currentPlatformIndex;
            Vector2Int last;
            while (TryGetLastIndex(current, out last))
            {
                current = last;
                if (PlatformAt(current).model.type == Model.PlatformType.SaveRing)
                    break;
            }
            CurrentPlatform = PlatformAt(current).view;
            CurrentPlatform.Visible = true;

            Vector2Int next;
            NextPlatform = TryGetNextIndex(current, out next) ? PlatformAt(next).view : null;

            m_currentPlatformIndex = current;
            m_nextPlatformIndex = next;
        }

        public void SetRedHeight(float height)
        {
            if (!m_redZoneObj.activeSelf)
                m_redZoneObj.SetActive(true);

            var pos = m_redZoneObj.transform.position;
            pos.y = height;
            m_redZoneObj.transform.position = pos;
        }

        private void ShiftLevels()
        {
            if (!endless || m_levels.Count == 0)
                return;

            int currentLevelIndex = m_levels[m_currentPlatformIndex.x].levelIndex;
            int lastLevelIndex = m_levels[m_levels.Count - 1].levelIndex;

            if (currentLevelIndex + 1 >= lastLevelIndex)
            {
                m_generator.GenerateLevel(ref m_lastGeneratedLevel, m_difficultGenerator.GetDifficult);
                if (m_levels.Count == 3)
                {
                    var levelWrapper = m_levels[0];
                    StartCoroutine(DestroyLevelCoroutine(levelWrapper));
                    m_map.RemoveLevel();
                    m_levels.RemoveAt(0);
                    m_currentPlatformIndex.x--;
                    m_nextPlatformIndex.x--;
                }
                CreateLevel(m_lastGeneratedLevel, true);
            }
        }

        private bool TryGetNextIndex(Vector2Int current, out Vector2Int next)
        {
            next = new Vector2Int();
            next.y = current.y >= 0 ? current.y + 1 : 0;
            next.x = current.x >= 0 ? current.x : 0;
            for ( ; next.x < m_levels.Count; next.x++, next.y = 0)
            {
                if (next.y < m_levels[next.x].platforms.Count)
                    return true;
            }
            return false;
        }

        private bool TryGetLastIndex(Vector2Int current, out Vector2Int last)
        {
            last = new Vector2Int();
            for (last.x = current.x; last.x >= 0; last.x--)
            {
                last.y = last.x == current.x ? current.y - 1 : m_levels[last.x].platforms.Count - 1;
                if (last.y >= 0)
                    return true;
            }
            return false;
        }

        public View.PlatformBase GetPlatformFromCurrent(int offset) {

            if (offset == 0)
                return CurrentPlatform;

            var index = m_currentPlatformIndex;
            int delta = offset;

            if (delta > 0) {
                for (; index.x < m_levels.Count; index.x++, index.y = 0) {
                    int count = m_levels[index.x].platforms.Count;
                    index.y += delta;

                    if (index.y < count)
                        return PlatformAt(index).view;

                    delta = index.y - count;
                }
            } else {
                bool first = true;
                for (; index.x >= 0; index.x--) {
                    if (!first)
                        index.y = m_levels[index.x].platforms.Count - 1;

                    first = false;
                    index.y += delta;

                    if (index.y >= 0)
                        return PlatformAt(index).view;

                    delta = index.y;
                }
            }
            return null;
        }

        public bool TryGetIndexFromCurrent(int offset, out Vector2Int index) {
            index = m_currentPlatformIndex;

            if (offset == 0)
                return false;

            int delta = offset;

            if (delta > 0) {
                for (; index.x < m_levels.Count; index.x++, index.y = 0) {
                    int count = m_levels[index.x].platforms.Count;
                    index.y += delta;

                    if (index.y < count)
                        return true;

                    delta = index.y - count;
                }
            }
            else {
                bool first = true;
                for (; index.x >= 0; index.x--) {
                    if (!first)
                        index.y = m_levels[index.x].platforms.Count - 1;

                    first = false;
                    index.y += delta;

                    if (index.y >= 0)
                        return true;

                    delta = index.y;
                }
            }
            return false;
        }

        private class LevelWrapper
        {
            public int levelIndex = 0;
            public GameObject pillar = null;
            public List<PlatformWrapper> platforms = new List<PlatformWrapper>();
        }

        private class PlatformWrapper
        {
            public Model.Platform model = null;
            public View.PlatformBase view = null;
        }

    }
}


