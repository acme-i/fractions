using System;

namespace fractions
{
    public static class ArrayExtensions
    {
        public static T[] ShuffleWith<T>(this T[] values, Random rand)
        {
            if (values?.Length > 1)
            {
                var length = values.Length;
                T temp;
                int b;
                for (var i = 0; i < length; i++)
                {
                    b = i;
                    while (b == i)
                    {
                        b = rand.Next(0, length);
                    }
                    temp = values[i];
                    values[i] = values[b];
                    values[b] = temp;
                }

            }
            return values;
        }

        public static int Next(this int[] values, Random rand)
        {
            return values[rand.Next(0, values.Length)];
        }

        public static uint Next(this uint[] values, Random rand)
        {
            return values[rand.Next(0, values.Length)];
        }

        public static float Next(this float[] values, Random rand)
        {
            return values[rand.Next(0, values.Length)];
        }

        public static double Next(this double[] values, Random rand)
        {
            return values[rand.Next(0, values.Length)];
        }

        public static Pitch Next(this Pitch[] values, Random rand)
        {
            return values[rand.Next(0, values.Length)];
        }

        public static Pitch? Next(this Pitch?[] values, Random rand)
        {
            return values[rand.Next(0, values.Length)];
        }
    }
}
