using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using fractions;
using fractions.ui.configuration;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace fractions.ui.viewmodels;

abstract public partial class EnumerateViewModel<T>(IMessenger messenger) : MessengerViewModel(messenger)
{
    [ObservableProperty]
    public Enumerate<T> source = new Enumerate<T>();

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

    public IncrementMethod Method
    {
        get => Source.Method;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Method));
            Source.Method = value;
            NotifyPropertyChangedOnUiThread(nameof(Method));
        }
    }

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
        NotifyPropertyChangedOnUiThread(nameof(IsReady));
        NotifyPropertyChangedOnUiThread(nameof(Source));
    }

    [RelayCommand(CanExecute = nameof(IsReady))]
    public void GoToNext()
    {
        NotifyPropertyChangingOnUiThread(nameof(Current));
        Source.GetNext();
        NotifyPropertyChangedOnUiThread(nameof(Current));
    }

    public T GetNext()
    {
        return Source.GetNext();
    }

    [RelayCommand(CanExecute=nameof(IsReady))]
    public void Clear()
    {
        Source.Clear();
    }
}
