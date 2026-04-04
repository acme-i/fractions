using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fractions;

namespace fractions.ui.viewmodels;

abstract public partial class EnumerateViewModel<T>() : BaseViewModel()
{
    [ObservableProperty]
    public Enumerate<T> source = new Enumerate<T>();

    public string Name
    {
        get => Source.Name;
        set
        {
            if(value != Source.Name)
            {
                NotifyPropertyChangingOnUiThread(nameof(Name));
                Source.Name = value;
                NotifyPropertyChangedOnUiThread(nameof(Name));
            }
        }
    }

    public IncrementMethod Method => Source.Method;
      
    public double Index
    {
        get => Source.Index;
    }

    public double StepSize
    {
        get => Source.StepSize;
    }

    public T Current => Source.Current;

    public T Peek => Source.Peek;

    [RelayCommand(CanExecute = nameof(IsReady))]
    public void PeekAt(int repeat)
    {
        PeekAtResult = Source.PeekAt(repeat);
    }

    [ObservableProperty]
    private T? peekAtResult;

    public bool IsReady()
    {
        return Source != null;
    }

    [RelayCommand(CanExecute = nameof(IsReady))]
    public void AddRange(IEnumerable<T> collection)
    {
        if (collection is null || collection.Any() == false)
        {
            return;
        }

        NotifyPropertyChangingOnUiThread(nameof(Source));
        Source.AddRange(collection);
        NotifyPropertyChangedOnUiThread(nameof(Source));
    }


    [RelayCommand(CanExecute = nameof(IsReady))]
    public void Set(IEnumerable<T> others)
    {
        NotifyPropertyChangingOnUiThread(nameof(Source));
        Source = new Enumerate<T>(others, Method, StepSize, Index, Guid.NewGuid().ToString());
        NotifyPropertyChangedOnUiThread(nameof(Source));
        NotifyPropertyChangedOnUiThread(nameof(IsReady));
    }

    [RelayCommand(CanExecute = nameof(IsReady))]
    public void GoToNext()
    {
        NotifyPropertyChangingOnUiThread(nameof(Current));
        Source.GetNext();
        NotifyPropertyChangedOnUiThread(nameof(Current));
    }

    [RelayCommand(CanExecute=nameof(IsReady))]
    public void Clear()
    {
        Source.Clear();
    }

    [RelayCommand]
    public void Reset()
    {
        NotifyPropertyChangingOnUiThread(nameof(Index));
        NotifyPropertyChangingOnUiThread(nameof(Current));
        Source.Reset();
        NotifyPropertyChangedOnUiThread(nameof(Index));
        NotifyPropertyChangedOnUiThread(nameof(Current));
    }
}
