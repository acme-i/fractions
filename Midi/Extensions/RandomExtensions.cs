using System;

namespace fractions
{
    public static class RandomExtensions
    {
        public static float NextVelocity(this Random random, float min = 0f, float max = 127f)
        {
            return (float)(((max - min) * random.NextDouble()) + min);
        }
    }
}
