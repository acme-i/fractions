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
        public static void ThrowIfTrue(bool condition, string message, string parameterName = null)
        {
            if (condition)
            {
                if (string.IsNullOrEmpty(parameterName))
                {
                    throw new ArgumentOutOfRangeException(message);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(parameterName, message);
                }
            }
        }
    }

    public static class ArgumentExceptionExtensions
    {
        public static void ThrowIfTrue(bool condition, string message, string parameterName = null)
        {
            if (condition)
            {
                if (string.IsNullOrEmpty(parameterName))
                {
                    throw new ArgumentException(message);
                }
                else
                {
                    throw new ArgumentException(parameterName, message);
                }
            }
        }
    }
}
