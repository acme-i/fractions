using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace fractions.ui.viewmodels;

public partial class FloatInterpolatorViewModel() : InterpolatorViewModel<float>()
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
            Values.Add(v);

        OnPropertyChanged(nameof(Values));
        IsBusy = false;
    }
}
