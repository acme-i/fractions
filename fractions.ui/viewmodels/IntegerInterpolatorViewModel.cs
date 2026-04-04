using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace fractions.ui.viewmodels;

public partial class IntegerInterpolatorViewModel() : InterpolatorViewModel<int>()
{
    [ObservableProperty] private float start = 0f;
    [ObservableProperty] private float end = 1f;
    public bool CanGenerate => IsNotBusy && Start < End && NumberOfSteps > 0 && Method >= 0;

    [RelayCommand(CanExecute = nameof(CanGenerate))]
    public void Generate()
    {
        OnPropertyChanging(nameof(Values));
        IsBusy = true;

        Values.Clear();
        foreach (var v in Interpolator.Interpolate(Start, End, NumberOfSteps, Method))
            Values.Add((int)Math.Round(v));

        OnPropertyChanged(nameof(Values));
        IsBusy = false;
    }
}