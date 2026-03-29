using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace fractions.ui.viewmodels;

public partial class DoubleInterpolatorViewModel : ObservableObject
{
    [ObservableProperty] private double start = 0f;
    [ObservableProperty] private double end = 1f;
    [ObservableProperty] private int numberOfSteps = 10;
    [ObservableProperty] private int method = 0;

    private bool isGenerating = false;

    public ObservableCollection<double> Values { get; } = [];

    [RelayCommand]
    public void Generate()
    {
        OnPropertyChanging(nameof(Values));
        isGenerating = true;

        Values.Clear();
        foreach (var v in Interpolator.Interpolate(Start, End, NumberOfSteps, Method))
            Values.Add(v);

        isGenerating = false;
        OnPropertyChanged(nameof(Values));
    }

    public bool CanGenerateMethod()
    {
        return Start < End && NumberOfSteps > 0 && Method >= 0 && !isGenerating;
    }
}