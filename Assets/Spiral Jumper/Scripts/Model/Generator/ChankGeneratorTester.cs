using System;
using System.Collections.Generic;
using UnityEngine;

using Rand = UnityEngine.Random;

namespace SpiralJumper.Model
{
    public class ChankGeneratorTester : MonoBehaviour
    {
        [SerializeField] private MapParams m_mapParams = new MapParams();
        [Space]
        public bool updateSeed = false;
        public bool run = false;
        [Space]
        public bool useSeed = false;
        public int seed = 0;
        [Space]
        public int count = 0;

        private ChankGenerator m_generator;


        private void Awake()
        {
            m_generator = new ChankGenerator(m_mapParams);
        }

        private void Update()
        {
            if (updateSeed)
            {
                updateSeed = false;
                UpdateSeed();
            }
            if (run)
            {
                run = false;
                Generate(seed);
            }
        }

        private void UpdateSeed()
        {
            if (!useSeed)
            {
                var timeSpan = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
                seed = new System.Random((int)timeSpan.TotalSeconds).Next(int.MinValue, int.MaxValue);
            }
        }


        private void Generate(int seed)
        {
            Rand.InitState(seed);

            string str = "GenerateChank:" + "\n";
            for (int i = 0; i < count; i++)
            {
                //var types = m_generator.GenerateChankPlatforms(m_mapParams.platformCountPerChank.x, m_mapParams.difficult);
                //str += types + "\n";
            }

            //foreach (var t in types)
            //    str += t.ToString() + "\n";
            //str += "\n";

            Debug.Log(str);
        }
    }
}