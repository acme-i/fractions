using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace fractions
{
    public static class EnumerateExtension
    {
        public static Enumerate<T> AsEnumeration<T>(this Enumerate<T> iEnumerable, int step = 1, int startIndex = 0, string name = null)
        {
            return new Enumerate<T>(iEnumerable, IncrementMethod.MinMax, step, startIndex, name);
        }

        public static Enumerate<T> AsEnumeration<T>(this IEnumerable<T> iEnumerable, IncrementMethod method = IncrementMethod.MinMax, int step = 1, int startIndex = 0, string name = null)
        {
            return new Enumerate<T>(iEnumerable, method, step, startIndex, name);
        }

        public static Enumerate<T> AsReversed<T>(this Enumerate<T> iEnumerable, int step = 1, int startIndex = 0, string name = null)
        {
            switch (iEnumerable.Method)
            {
                case IncrementMethod.MinMax:
                    return new Enumerate<T>(iEnumerable, IncrementMethod.MaxMin, step, startIndex, name + "Reversed");

                case IncrementMethod.MaxMin:
                    return new Enumerate<T>(iEnumerable, IncrementMethod.MinMax, step, startIndex, name + "Reversed");

                case IncrementMethod.Cyclic:
                    return new Enumerate<T>(iEnumerable.Reverse<T>(), IncrementMethod.Cyclic, step, startIndex, name + "Reversed");

                case IncrementMethod.Bit:
                    return new Enumerate<T>(iEnumerable.Reverse<T>(), IncrementMethod.Bit, step, startIndex, name + "Reversed");

                default:
                    return new Enumerate<T>(iEnumerable, iEnumerable.Method, step, startIndex, name + "Reversed");
            }
            
        }

        public static Enumerate<T> AsMaxMinEnumeration<T>(this Enumerate<T> iEnumerable, int step = 1, int startIndex = 0, string name = null)
        {
            return new Enumerate<T>(iEnumerable, IncrementMethod.MaxMin, step, startIndex, name);
        }

        public static Enumerate<T> AsMaxMinEnumeration<T>(this IEnumerable<T> iEnumerable, int step = 1, int startIndex = 0, string name = null)
        {
            return new Enumerate<T>(iEnumerable, IncrementMethod.MaxMin, step, startIndex, name);
        }

        public static Enumerate<T> AsCycle<T>(this Enumerate<T> iEnumerable, int step = 1, int startIndex = 0, string name = null)
        {
            return new Enumerate<T>(iEnumerable, IncrementMethod.Cyclic, step, startIndex, name);
        }

        public static Enumerate<T> AsCycle<T>(this IEnumerable<T> iEnumerable, int step = 1, int startIndex = 0, string name = null)
        {
            return new Enumerate<T>(iEnumerable, IncrementMethod.Cyclic, step, startIndex, name);
        }

        /// <returns>Scales by amount</returns>
        public static Enumerate<double> Scale(this Enumerate<double> input, double amount)
        {
            return (input?.Any() == false || amount == 1)
                ? input
                : new Enumerate<double>(
                    input.Select(p => p * amount),
                    input.Method,
                    input.StepSize,
                    input.Index,
                    input.Name
                );
        }

        /// <returns>Scales by amount</returns>
        public static Enumerate<float> Scale(this Enumerate<float> input, double amount)
        {
            return (input?.Any() == false || amount == 1)
                ? input
                : new Enumerate<float>(
                    input.Select(p => (float)(p * amount)),
                    input.Method,
                    input.StepSize,
                    input.Index,
                    input.Name
                );
        }

        /// <returns>Scales by amount</returns>
        public static Enumerate<int> Scale(this Enumerate<int> input, double amount)
        {
            return (input?.Any() == false || amount == 1)
                ? input
                : new Enumerate<int>(
                    input.Select(p => (int)(p * amount)),
                    input.Method,
                    input.StepSize,
                    input.Index,
                    input.Name
                );
        }
    }
}
