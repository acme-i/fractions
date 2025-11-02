using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fractions
{
    public static class CharExtensions
    {
        public static void ThrowIfOutOfRange(this char letter, string parameterName)
        {
            ArgumentOutOfRangeExceptionExtensions.ThrowIfTrue(
                letter < 'A' || letter > 'G',
                $"Note must be >= A and <= G, but was {letter}",
                parameterName
            );
        }
    }
}
