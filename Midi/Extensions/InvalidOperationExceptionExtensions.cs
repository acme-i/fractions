using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fractions
{
    public static class InvalidOperationExceptionExtensions
    {
        public static void ThrowIfTrue(bool condition, string message)
        {
            if (condition) throw new InvalidOperationException(message);
        }
    }

    public static class ArgumentOutOfRangeExceptionExtensions
    {
        public static void ThrowIfTrue(bool condition, string message)
        {
            if (condition)
                throw new ArgumentOutOfRangeException(message);
        }
    }
}
