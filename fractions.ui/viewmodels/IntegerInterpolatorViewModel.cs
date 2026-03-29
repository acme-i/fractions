using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fractions.ui.viewmodels;

public partial class IntegerInterpolatorViewModel : ObservableObject
{
    [ObservableProperty] private int start = 0;
    [ObservableProperty] private int end = 127;
    [ObservableProperty] private int numberOfSteps = 10;
    [ObservableProperty] private int method = 0;

    public ObservableCollection<int> Values { get; } = [];

    private bool isGenerating = false;

    [RelayCommand(CanExecute = nameof(CanGenerateMethod))]
    public void Generate()
    {
        OnPropertyChanging(nameof(Values));
        isGenerating = true;

        Values.Clear();
        foreach (var v in Interpolator.Interpolate(Start, End, NumberOfSteps, Method))
            Values.Add((int)Math.Round(v));

        isGenerating = false;
        OnPropertyChanged(nameof(Values));
    }

    public bool CanGenerateMethod()
    {
        return Start < End && NumberOfSteps > 0 && Method >= 0 && !isGenerating;
    }
}