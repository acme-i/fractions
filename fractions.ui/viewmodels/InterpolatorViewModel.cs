using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace fractions.ui.viewmodels;

public partial class InterpolatorViewModel<T>() : BaseViewModel()
{
    [ObservableProperty] private int numberOfSteps = 10;
    [ObservableProperty] private int method = 0;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy = false;
    internal bool IsNotBusy => !IsBusy;
    public ObservableCollection<T> Values { get; } = [];

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    public void Clear() => Values.Clear();
}
