using System;

namespace fractions
{
    public interface IRange<T>
    {
        T Min { get; set;  }
        T Max { get; set; }
    }

    public interface IAssertRange<T>
        where T : IComparable
    {
        void Assert(T min, T max);
    }
}