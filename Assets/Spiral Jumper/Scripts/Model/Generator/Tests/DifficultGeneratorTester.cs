using System;
using System.Collections.Generic;
using UnityEngine;

using SpiralJumper.Model;

namespace SpiralJumper
{
    [ExecuteInEditMode]
    public class DifficultGeneratorTester : MonoBehaviour
    {
        public DifficultParams difficultParams;

        public bool run = false;


        private void Update()
        {
            if (run)
            {
                run = false;
                Run();
            }
        }


        private void Run()
        {
            DifficultGenerator generator = new DifficultGenerator(difficultParams);

            string str = "Difficult Generator Tester:\n";

            for (int level = 0; level < 100; level++)
            {
                str += "Level: " + level.ToString() + "\n";

                int chanksCount = 5;
                for (int chank = 0; chank < chanksCount; chank++)
                {
                    float d = generator.GetDifficult(level, chank, chanksCount);
                    str += "ch " + chank.ToString() + ": " + d.ToString() + "\n";
                }
                str += "---\n";
            }

            Debug.Log(str);
        }
    }
}