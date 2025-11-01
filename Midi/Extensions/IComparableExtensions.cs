using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace fractions
{
    public static class IComparableExtensions
    {
        public static void ThrowIfLessThanOrEqualTo<T>(this IComparable<T> value, T maxValue, string parameterName)
            where T : IComparable
        {
            if (value.CompareTo(maxValue) <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"The value must be greater than {maxValue}, but was {value}");
            }
        }

        public static void ThrowIfOutOfRange<T>(this IComparable<T> value, T minValue, T maxValue, string parameterName)
            where T : IComparable
        {
            if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 1)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"The value must be greater than, or equal to, {minValue} and smaller than, or equal to, {maxValue}, but was {value}");
            }
        }

        public static void ThrowIfCountIsLessThan<T>(this IEnumerable<T> collection, int minCount, string parameterName)
        {
            if (collection.Count() < minCount)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"collection must contain at least {minCount} items, but had {collection.Count()}");
            }
        }
    }
}
