using System;

namespace DiGro.Utils
{
    public static class RandomExtentions
    {
        /// <summary>
        /// Return a random integer number between min [inclusive] and max [exclusive]
        /// </summary>
        public static int Range(this Random random, int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// Return a random float number between min [inclusive] and max [inclusive]
        /// </summary>
        public static float Range(this Random random, float min, float max)
        {
           return min + (float)(random.NextDouble() * (max - min));
        }
    }
}