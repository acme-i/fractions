using System;
using System.Collections.Generic;
using System.Linq;

namespace fractions
{
    public static class EnumerateExtension
    {
        /// <returns>Scales by amount</returns>
        public static Enumerate<float> Scale(this Enumerate<float> input, double amount)
        {
            if (input?.Any() == false || amount == 1)
                return input;
            var coll = input.Select(p => (float)Math.Round(p * amount));
            return new Enumerate<float>(coll, input.Incrementor);
        }

        /// <returns>Scales by amount</returns>
        public static Enumerate<int> Scale(this Enumerate<int> input, double amount)
        {
            if (input?.Any() == false || amount == 1)
                return input;
            var coll = input.Select(p => (int)Math.Round(p * amount));
            return new Enumerate<int>(coll, input.Incrementor);
        }
    }
}
