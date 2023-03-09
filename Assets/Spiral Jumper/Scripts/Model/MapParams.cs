using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper.Model
{
    [Serializable]
    public class MapParams
    {
        public int levelsCount = 1;
        public int chanksPerLevel = 5;
        public int chanksPerDiffLevel = 5;
        [Space]
        public Vector2Int platformCountPerChank = new Vector2Int(4, 7);
        public Vector2 heightBetweenPlatforms = new Vector2(0.3f, 0.8f);
        public Vector2 angleBetweenPlatforms = new Vector2(10f, 45f);
        [Space]
        public PlatformProbabilities probabilities = new PlatformProbabilities();
        [Space]
        public float startRedZoneHeight = -1;
        [Space]
        public bool useSeed = false;
        public int seed;
        [Space]
        public DifficultParams difficultParams = null;

        public MapParams Clone() {
            var clone = new MapParams();

            clone.levelsCount = levelsCount;
            clone.chanksPerLevel = chanksPerLevel;
            clone.chanksPerDiffLevel = chanksPerDiffLevel;
            clone.platformCountPerChank = platformCountPerChank;
            clone.heightBetweenPlatforms = heightBetweenPlatforms;
            clone.angleBetweenPlatforms = angleBetweenPlatforms;
            clone.probabilities = probabilities;
            clone.startRedZoneHeight = startRedZoneHeight;
            clone.useSeed = useSeed;
            clone.seed = seed;
            clone.difficultParams = difficultParams;

            return clone;
        }
    }
}