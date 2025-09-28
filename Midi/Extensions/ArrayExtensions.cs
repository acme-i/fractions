using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fractions
{
    public static class ArrayExtensions
    {
        /// <summary>
        ///     Fills dest with a rotated version of source.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="dest">
        ///     The dest array, which must have the same length and underlying type
        ///     as source.
        /// </param>
        /// <param name="rotation">The number of elements to rotate to the left by.</param>
        public static void RotateArrayLeft(this Array source, Array dest, int rotation)
        {
            if (source.Length != dest.Length)
            {
                throw new ArgumentException("source and dest lengths differ.");
            }
            if (rotation == 0)
            {
                source.CopyTo(dest, 0);
            }
            else
            {
                for (var i = 0; i < source.Length; ++i)
                {
                    dest.SetValue(source.GetValue((rotation + i) % source.Length), i);
                }
            }
        }

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
