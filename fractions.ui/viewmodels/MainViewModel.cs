using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using fractions.ui.views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fractions.ui.viewmodels;

public partial class MainViewModel : BaseViewModel
{
    public MainViewModel(IMessenger messenger, Settings settings, OutputDevicesViewModel outputDevicesViewModel, ClockViewModel clockViewModel, NoteOnOffListViewModel noteOnOffListViewModel) : base(messenger, settings)
    {
        _outputDevicesViewModel = outputDevicesViewModel;
        _clockViewModel = clockViewModel;
        _noteOnOffListViewModel = noteOnOffListViewModel;
    }

    [ObservableProperty]
    private OutputDevicesViewModel _outputDevicesViewModel;

    [ObservableProperty]
    private ClockViewModel _clockViewModel;

    [ObservableProperty]
    private NoteOnOffListViewModel _noteOnOffListViewModel;


}