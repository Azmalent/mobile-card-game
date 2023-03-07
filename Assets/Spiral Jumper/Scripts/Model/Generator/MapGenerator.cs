using System;
using System.Collections.Generic;
using UnityEngine;

using DiGro.Utils;

using Rand = System.Random;


namespace SpiralJumper.Model
{
    public class MapGenerator
    {
        public delegate float Diff(int level, int chank, int chankCount);

        private Rand m_rand;
        private int m_seed;

        private IMap m_map;
        private ChankGenerator m_chankGenerator;


        public MapGenerator(IMap map)
        {
            m_map = map;

            if (m_map.MapParams.useSeed)
            {
                m_seed = m_map.MapParams.seed;
            }
            else
            {
                var timeSpan = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
                m_seed = new Rand((int)timeSpan.TotalSeconds).Next(int.MinValue, int.MaxValue);
            }
            m_map.Seed = m_seed;
            m_rand = new Rand(m_seed);

            m_chankGenerator = new ChankGenerator(m_map.MapParams);
            m_chankGenerator.rand = m_rand;
        }


        public void Generate(int levelsCount, Diff getDifficult) 
        {
            Level previewsLevel = null;
            for (int i = 0; i < levelsCount; i++)
                GenerateLevel(ref previewsLevel, getDifficult);
        }

        public void GenerateLevel(ref Level previewsLevel, Diff getDifficult)
        {
            var level = new Level();
            var previewsChank = (previewsLevel == null || previewsLevel.chanks.Count == 0) ? null : previewsLevel.LastChank;

            level.beginHeight = previewsChank == null ? 0 : previewsChank.endHeight;

            float difficulty = 0;

            int chanksCount = m_map.MapParams.chanksPerLevel;
            int diffChanksCount = m_map.MapParams.chanksPerDiffLevel;

            for (int i = 0; i < chanksCount; i++)
            {
                //var chankDifficulty = getDifficult(m_map.GeneratedLevelsCount, i, chanksCount);
                int diffChankIndex = m_map.GeneratedDiffChanksCount % diffChanksCount;
                var chankDifficulty = getDifficult(m_map.GeneratedDiffLevelsCount, diffChankIndex, diffChanksCount);
                m_map.GeneratedDiffChanksCount++;

                var chank = GenerateChank(chankDifficulty, previewsChank);
                level.chanks.Add(chank);

                difficulty += chank.difficulty;

                if (previewsChank == null)
                {
                    chank.platforms.Insert(0, new Platform
                    {
                        type = PlatformType.SaveRing,
                        height = chank.beginHeight,
                        angle = 0
                    });
                }
                if (i == chanksCount - 1)
                {
                    float heightDelta = m_rand.Range(m_map.MapParams.heightBetweenPlatforms.x, m_map.MapParams.heightBetweenPlatforms.y);
                    chank.endHeight = chank.endHeight + heightDelta;
                    chank.platforms.Add(new Platform
                    {
                        height = chank.endHeight,
                        angle = chank.HasPlatforms ? chank.LastPlatform.angle : 0,
                        type = PlatformType.SaveRing
                    });
                }
                previewsChank = chank;
            }

            level.difficulty = difficulty / chanksCount;
            level.endHeight = previewsChank == null ? 0 : previewsChank.endHeight;
            level.Index = m_map.GeneratedLevelsCount;
            m_map.GeneratedLevelsCount++;

            m_map.AddLevel(level);
            m_map.EndHeight = level.endHeight;

            previewsLevel = level;
        }

        private Chank GenerateChank(float targetDifficulty, Chank previewsChank = null)
        {
            var chank = new Chank();
            var previewsPlatform = (previewsChank == null || !previewsChank.HasPlatforms) ? null : previewsChank.LastPlatform;

            int platformCount = m_rand.Range(m_map.MapParams.platformCountPerChank.x, m_map.MapParams.platformCountPerChank.y);

            chank.beginHeight = previewsChank == null ? 0 : previewsChank.endHeight;
            chank.endHeight = chank.beginHeight;

            float lastHeight = chank.beginHeight;
            float lastAngle = previewsPlatform == null ? 0 : previewsPlatform.angle + MapFactory.get.PlatformInfo(previewsPlatform.type).length;

            //var chankDifficulty = m_rand.Range(targetDifficulty - 1, targetDifficulty + 1);
            var platformTypes = m_chankGenerator.GenerateChankPlatforms(platformCount, targetDifficulty);

            float difficulty = 0;
            for (int i = 0; i < platformCount; i++)
            {
                var platform = new Platform();

                float heightDelta = m_rand.Range(m_map.MapParams.heightBetweenPlatforms.x, m_map.MapParams.heightBetweenPlatforms.y);
                float angleDelta = m_rand.Range(m_map.MapParams.angleBetweenPlatforms.x, m_map.MapParams.angleBetweenPlatforms.y);

                platform.height = lastHeight + heightDelta;
                platform.angle = lastAngle + angleDelta;
                platform.type = platformTypes[i];

                lastHeight = platform.height;
                lastAngle = platform.angle + MapFactory.get.PlatformInfo(platform.type).length;

                difficulty += MapFactory.get.PlatformInfo(platform.type).difficulty;

                chank.endHeight = platform.height;
                chank.platforms.Add(platform);
            }

            chank.difficulty = difficulty / platformCount;
            return chank;
        }


        private PlatformType GetPlatformType(List<PlatformProbabilities.PropabilityRange> ranges)
        {
            var p = (float) m_rand.NextDouble();
            foreach (var range in ranges)
            {
                if (p > range.p1 && p <= range.p2)
                    return range.type;
            }
            throw new Exception("Propability Ranges not contains range for propability equals " + p.ToString() + ".");
        }

    }
}