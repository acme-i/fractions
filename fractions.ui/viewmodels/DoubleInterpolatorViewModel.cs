using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace fractions.ui.viewmodels;

public partial class DoubleInterpolatorViewModel() : InterpolatorViewModel<double>()
{
    [ObservableProperty] private double start = 0d;
    [ObservableProperty] private double end = 1d;

    public bool CanGenerate => IsNotBusy && Start < End && NumberOfSteps > 0 && Method >= 0;

    [RelayCommand(CanExecute = nameof(CanGenerate))]
    public void Generate()
    {
        OnPropertyChanging(nameof(Values));
        IsBusy = true;

        Values.Clear();
        foreach (var v in Interpolator.Interpolate(Start, End, NumberOfSteps, Method))
            Values.Add(v);

        OnPropertyChanged(nameof(Values));
        IsBusy = false;
    }
}