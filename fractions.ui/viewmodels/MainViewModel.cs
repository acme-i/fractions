using CommunityToolkit.Mvvm.ComponentModel;

namespace fractions.ui.viewmodels;

public partial class MainViewModel : BaseViewModel
{
    public MainViewModel(
        OutputDevicesViewModel outputDevicesViewModel, 
        ClockViewModel clockViewModel, 
        NoteOnOffListViewModel noteOnOffListViewModel,
        FloatInterpolatorViewModel floatInterpolatorViewModel
        )
        : base()
    {
        _outputDevicesViewModel = outputDevicesViewModel;
        _clockViewModel = clockViewModel;
        _noteOnOffListViewModel = noteOnOffListViewModel;
        _floatInterpolatorViewModel = floatInterpolatorViewModel;
    }

    [ObservableProperty]
    private OutputDevicesViewModel _outputDevicesViewModel;

    [ObservableProperty]
    private ClockViewModel _clockViewModel;

    [ObservableProperty]
    private NoteOnOffListViewModel _noteOnOffListViewModel;

    [ObservableProperty]
    private FloatInterpolatorViewModel _floatInterpolatorViewModel;
}