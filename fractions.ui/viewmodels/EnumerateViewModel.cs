using CommunityToolkit.Mvvm.Messaging;
using fractions;
using System.Linq;
using System.Collections.Generic;
using fractions.ui.configuration;

namespace fractions.ui.viewmodels;

abstract public class EnumerateViewModel<T>(Enumerate<T> source) : BaseObservableObject(), IEnumerateViewModelOfT<T>
{
    public Enumerate<T> Source { get; } = source;

    public Incrementor Incrementor => this.Source.Incrementor;

    public string Name
    {
        get => Source.Name;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Name));
            Source.Name = value;
            NotifyPropertyChangedOnUiThread(nameof(Name));
        }
    }

    public int Length
    {
        get => Source.Length;
    }

    public T Current => Source.Current;

    public T Max => Source.Max;

    public T Min => Source.Min;

    public T Peek => Source.Peek;

    public void AddRange(IEnumerable<T> others)
    {
        NotifyPropertyChangingOnUiThread(nameof(Source));
        Source.AddRange(others);
        NotifyPropertyChangedOnUiThread(nameof(Source));
    }

    public void Set(IEnumerable<T> others)
    {
        NotifyPropertyChangingOnUiThread(nameof(Source));
        Source.Set(others);
        NotifyPropertyChangedOnUiThread(nameof(Source));
    }

    public T GetNext() => Source.GetNext();

    public T PeekAt(int steps) => Source.PeekAt(steps);

    public override Task<int> Reload(string? filter = null) => Task.FromResult(0);

}
