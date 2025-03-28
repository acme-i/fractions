using System;
using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    /// <summary>
    /// EO: 2016-04-14
    /// Generator of all permutations of an array of anything.
    /// Base on Heap's Algorithm. See: https://en.wikipedia.org/wiki/Heap%27s_algorithm#cite_note-3
    /// </summary>
    public static class Permutations
    {
        /// <summary>
        /// Heap's algorithm to find all permutations. Non recursive, more efficient.
        /// </summary>
        /// <param name="items">Items to permute in each possible ways</param>
        /// <param name="shouldStop"></param>
        /// <returns>Return true if cancelled</returns> 
        public static bool PermutationsOf<T>(T[] items, Func<T[], bool> shouldStop)
        {
            int countOfItem = items.Length;

            if (countOfItem <= 1)
            {
                return shouldStop(items);
            }

            var indexes = new int[countOfItem];
            for (int i = 0; i < countOfItem; i++)
            {
                indexes[i] = 0;
            }

            if (shouldStop(items))
            {
                return true;
            }

            for (int i = 1; i < countOfItem;)
            {
                if (indexes[i] < i)
                { // On the web there is an implementation with a multiplication which should be less efficient.
                    if ((i & 1) == 1) // if (i % 2 == 1)  ... more efficient ??? At least the same.
                    {
                        Swap(ref items[i], ref items[indexes[i]]);
                    }
                    else
                    {
                        Swap(ref items[i], ref items[0]);
                    }

                    if (shouldStop(items))
                    {
                        return true;
                    }

                    indexes[i]++;
                    i = 1;
                }
                else
                {
                    indexes[i++] = 0;
                }
            }

            return false;
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> enumerable)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();

            var factorials = Enumerable.Range(0, array.Length + 1)
                .Select(Factorial)
                .ToArray();

            for (var i = 0L; i < factorials[array.Length]; i++)
            {
                var sequence = GenerateSequence(i, array.Length - 1, factorials);

                yield return GeneratePermutation(array, sequence);
            }
        }

        private static IEnumerable<T> GeneratePermutation<T>(T[] array, IList<int> sequence)
        {
            var clone = (T[])array.Clone();

            for (int i = 0; i < clone.Length - 1; i++)
            {
                Swap(ref clone[i], ref clone[i + sequence[i]]);
            }

            return clone;
        }

        private static int[] GenerateSequence(long number, int size, IList<long> factorials)
        {
            var sequence = new int[size];

            for (var j = 0; j < sequence.Length; j++)
            {
                var facto = factorials[sequence.Length - j];

                sequence[j] = (int)(number / facto);
                number = (int)(number % facto);
            }

            return sequence;
        }

        private static long Factorial(int n)
        {
            long result = n;

            for (int i = 1; i < n; i++)
            {
                result *= i;
            }

            return result;
        }

        /// <summary>
        /// Swap 2 elements of same type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        static void Swap<T>(ref T a, ref T b)
        {
            (b, a) = (a, b);
        }
    }
}
