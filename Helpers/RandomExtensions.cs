using System;

namespace fractions
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns whether the collection has any members.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns>whether the collection has any members</returns>
        public static float NextVelocity(this Random random, float min = 0f, float max = 127f)
        {
            return (float)(((max - min) * random.NextDouble()) + min);
        }
    }
}
