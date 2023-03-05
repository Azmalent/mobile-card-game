using System;
using System.Collections.Generic;
using UnityEngine;


namespace SpiralJumper.Model
{
    public class DifficultGenerator
    {
        private DifficultParams m_params;

        public DifficultGenerator(DifficultParams dParams)
        {
            m_params = dParams;
        }

        public float GetDifficult(int level, int chank, int chanksCount)
        {

            int bigLevelOffset = m_params.firstBigLoop.length;
            DifficultParams.DifficultLoop bigLoop = m_params.bigLoop;
            if (level < m_params.firstBigLoop.length)
            {
                bigLoop = m_params.firstBigLoop;
                bigLevelOffset = 0;
            }

            float levelProgress = chank / (float)chanksCount;

            float smallValue = m_params.smallLoop.curve.Evaluate(levelProgress);
            float smallDelta = m_params.smallLoop.max - m_params.smallLoop.min;
            float small = m_params.smallLoop.min + smallValue * smallDelta;

            float p = levelProgress / bigLoop.length;
            float bigProgress = (level - bigLevelOffset) % bigLoop.length / (float)bigLoop.length;
            float bigValue = bigLoop.curve.Evaluate(bigProgress + p);
            float bigDelta = bigLoop.max - bigLoop.min;
            float big = bigLoop.min + bigValue * bigDelta;

            return big + small;
        }
    }
}