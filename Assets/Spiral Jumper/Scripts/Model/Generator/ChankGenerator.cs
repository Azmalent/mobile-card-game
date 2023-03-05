using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using DiGro.Utils;

using Rand = System.Random;


namespace SpiralJumper.Model
{
    public class ChankGenerator
    {
        public Rand rand { get; set; }

        private List<PlatformType> m_types = new List<PlatformType>();
        private List<float> m_prices = new List<float>();
        private List<Pair<float, int>> m_sortedPrices = new List<Pair<float, int>>();

        private float m_maxAverageSum;
        private float m_minAverageSum;
        private float m_target = 1;
        private float m_accuracy = 0.2f;

        public ChankGenerator(MapParams mapParams)
        {
            var infos = MapFactory.get.platformsInfo.list;
            for (int i = 0; i < infos.Count; i++)
            {
                var info = infos[i];
                if (info.canGenerate)
                {
                    m_sortedPrices.Add(Pair.Make(info.difficulty, m_types.Count));
                    m_types.Add(info.type);
                    m_prices.Add(info.difficulty);
                }
            }
            m_sortedPrices.Sort((Pair<float, int> p1, Pair<float, int> p2) => p1.first.CompareTo(p2.first));

            m_maxAverageSum = m_sortedPrices[m_sortedPrices.Count - 1].first;
            m_minAverageSum = m_sortedPrices[0].first;
        }


        public List<PlatformType> GenerateChankPlatforms(int platformCount, float targetDifficulty)
        {
            m_target = targetDifficulty;

            var types = new List<PlatformType>(new PlatformType[platformCount]);
            var diff = new List<float>(new float[platformCount]);

            for (int i = 0; i < diff.Count; i++)
            {
                int index = rand.Range(0, m_prices.Count);
                types[i] = m_types[index];
                diff[i] = m_prices[index];
            }

            var sortDelta = new List<float>(m_sortedPrices.Count);
            for (int i = 0; i < m_sortedPrices.Count; i++)
                sortDelta.Add(m_sortedPrices[i].first / platformCount);

           
            m_accuracy = sortDelta[0];

            int iters = 0;
            while (iters < 100 && !IsFound(diff))
            {
                var difference = Difference(diff);
                if (difference < 0)
                {
                    var min = diff.Min();
                    var resIndex = diff.LastIndexOf(min);
                    var firstPriceIndex = m_sortedPrices.FindIndex((Pair<float, int> p) => p.first > min);
                    var lastPriceIndex = m_sortedPrices.FindLastIndex((Pair<float, int> p) => p.first > min);
                    if (firstPriceIndex >= 0 && firstPriceIndex >= 0)
                    {
                        var pair = m_sortedPrices[rand.Range(firstPriceIndex, lastPriceIndex + 1)];
                        types[resIndex] = m_types[pair.second];
                        diff[resIndex] = pair.first;
                    }
                }
                else
                {
                    var max = diff.Max();
                    var resIndex = diff.IndexOf(max);
                    var firstPriceIndex = m_sortedPrices.FindIndex((Pair<float, int> p) => p.first < max);
                    var lastPriceIndex = m_sortedPrices.FindLastIndex((Pair<float, int> p) => p.first < max);
                    if (firstPriceIndex >= 0 && firstPriceIndex >= 0)
                    {
                        var pair = m_sortedPrices[rand.Range(firstPriceIndex, lastPriceIndex + 1)]; 
                        types[resIndex] = m_types[pair.second];
                        diff[resIndex] = pair.first;
                    }
                }
                iters++;
            }

            //var av = diff.Sum() / diff.Count;

            //string str = "";
            //str += diff.ToString(", ", "{", "}");
            //str += " av: " + av.ToString();
            //str += " iters: " + iters.ToString();

            //Debug.Log(str);
            return types;
        }

        private bool IsFound(IList<float> list)
        {
            float average = list.Sum() / list.Count;
            bool min = m_target <= m_minAverageSum && average <= m_minAverageSum;
            bool max = m_target >= m_maxAverageSum && average >= m_maxAverageSum;
            return min || max || Math.Abs(Difference(list)) < m_accuracy;
        }

        private float Difference(IList<float> list)
        {
            return list.Sum() / list.Count - m_target;
        }
    }
}