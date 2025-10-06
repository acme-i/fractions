using System;

namespace fractions
{
    public abstract class EnumRange<T> : IRange<T>, IAssertRange<T>
        where T : IComparable
    {
        public EnumRange(T min, T max)
        {
            this.min = min;
            this.max = max;
            Assert(min, max);
        }

        private T min;
        public T Min
        {
            get => min;
            set
            {
                Assert(value, max);
                min = value;
            }
        }

        private T max;
        public T Max
        {
            get => max;
            set
            {
                Assert(min, value);
                max = value;
            }
        }

        public abstract void Assert(T min, T max);
    }
}