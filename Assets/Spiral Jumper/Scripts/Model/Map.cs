using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper.Model
{
    public abstract class IMap
    {
        public MapParams MapParams { get; private set; }
        public int GeneratedLevelsCount { get; set; }
        public float BeginHeight { get; set; }
        public float EndHeight { get; set; }
        ///public float Difficult { get; set; }
        public int Seed { get; set; }

        public abstract int LevelsCount { get; }

        public abstract void AddLevel(Level level);
        public abstract Level GetLevel(int index);

        public abstract IEnumerator<Level> GetEnumerator();

        public IMap(MapParams mapParams) => MapParams = mapParams;
    }

    public class Map : IMap
    {
        public override int LevelsCount => m_levels.Count;

        public override void AddLevel(Level level) => m_levels.Add(level);
        public override Level GetLevel(int index) => m_levels[index];

        public override IEnumerator<Level> GetEnumerator() => m_levels.GetEnumerator();

        private List<Level> m_levels = new List<Level>();


        public Map(MapParams mapParams) : base(mapParams) { }
    }

    public class EndlessMap : IMap
    {
        public override int LevelsCount => m_levels.Count;
        public override void AddLevel(Level level) => m_levels.Enqueue(level);
        public override Level GetLevel(int index) => m_levels.ElementAt(index);

        public override IEnumerator<Level> GetEnumerator() => m_levels.GetEnumerator();

        public void RemoveLevel()
        {
            if (m_levels.Count > 0)
                m_levels.Dequeue();
        }

        private Queue<Level> m_levels = new Queue<Level>();


        public EndlessMap(MapParams mapParams) : base(mapParams) { }
    }
}