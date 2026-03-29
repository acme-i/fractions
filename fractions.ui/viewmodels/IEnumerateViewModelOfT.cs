using System;
using System.Collections.Generic;

namespace fractions.ui.viewmodels
{
    public interface IEnumerateViewModelOfT<T>
    {
        string Name { get; set;  }
        int Count { get; }
        void AddRange(IEnumerable<T> others);
        void Set(IEnumerable<T> others);
        T Current { get; }
        T Max { get; }
        T Min { get; }
        T GetNext();
        T Peek { get; }
        T PeekAt(int steps);
        int Index { get; }
        int PreviousIndex { get; }
    }
}