using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace fractions.ui.viewmodels;

public partial class FloatInterpolatorViewModel : ObservableObject
{
    [ObservableProperty] private float start = 0f;
    [ObservableProperty] private float end = 1f;
    [ObservableProperty] private int numberOfSteps = 10;
    [ObservableProperty] private int method = 0;

    public ObservableCollection<float> Values { get; } = [];

    private bool isGenerating = false;

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