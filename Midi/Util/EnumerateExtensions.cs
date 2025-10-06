using System;
using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    public static class EnumerateExtension
    {
        public static Enumerate<T> AsEnumeration<T>(this IEnumerable<T> iEnumerable, IncrementMethod method = IncrementMethod.MinMax, int step = 1, int startIndex = 0, string name = null)
        {
            return new Enumerate<T>(iEnumerable, method, step, startIndex, name);
        }

        public static Enumerate<T> AsEnumeration<T>(this IEnumerable<T> iEnumerable, Incrementor incrementor, string name = null)
        {
            return new Enumerate<T>(iEnumerable, incrementor, name);
        }

        public static Enumerate<T> AsCycle<T>(this IEnumerable<T> iEnumerable, int step = 1, int startIndex = 0)
        {
            return new Enumerate<T>(iEnumerable, IncrementMethod.Cyclic, step, startIndex);
        }

        public static Enumerate<T> AsReversed<T>(this Enumerate<T> enumerate)
        {
            return new Enumerate<T>(enumerate.collection.Reverse(), enumerate.Incrementor);
        }

        /// <returns>Scales by amount</returns>
        public static Enumerate<double> Scale(this Enumerate<double> input, double amount)
        {
            input.Select(p => p * amount).AsEnumeration(input.Incrementor);

            return (input?.Any() == false || amount == 1)
                ? input
                : new Enumerate<double>(
                    input.Select(p => p * amount),
                    input.Incrementor,
                    $"{input.Name}*{amount:2}"
                );
        }

        /// <returns>Scales by amount</returns>
        public static Enumerate<float> Scale(this Enumerate<float> input, double amount)
        {
            return (input?.Any() == false || amount == 1)
                ? input
                : new Enumerate<float>(
                    input.Select(p => (float)Math.Round(p * amount)), 
                    input.Incrementor, 
                    $"{input.Name}*{amount:2}"
                );
        }

        /// <returns>Scales by amount</returns>
        public static Enumerate<int> Scale(this Enumerate<int> input, double amount)
        {
            return (input?.Any() == false || amount == 1)
                ? input
                : new Enumerate<int>(
                    input.Select(p => (int)Math.Round(p * amount)), 
                    input.Incrementor, 
                    $"{input.Name}*{amount:2}"
                );
        }
    }
}
