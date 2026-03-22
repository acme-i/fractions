using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace fractions.ui.viewmodels;

public partial class OutputDevicesViewModel : BaseObservableObject
{
    public OutputDevicesViewModel(IOutputDevice outputDevice)
    {
        _outputDevice = outputDevice;
    }

    private IOutputDevice _outputDevice;

    /// <summary>
    /// The selected channel value.
    /// </summary>
    public IOutputDevice SelectedOutputDevice
    {
        get => _outputDevice;
        set
        {
            if (_outputDevice != value)
            {
                NotifyPropertyChangingOnUiThread(nameof(OutputDevice));
                NotifyPropertyChangingOnUiThread(nameof(DisplayName));
                _outputDevice = value;
                NotifyPropertyChangedOnUiThread(nameof(OutputDevice));
                NotifyPropertyChangedOnUiThread(nameof(DisplayName));
            }
        }
    }

    /// <summary>
    /// Display-friendly name for the channel (e.g., "Channel1").
    /// </summary>
    public string DisplayName
    {
        get
        {
            return _outputDevice.Name;
        }
    }

    /// <summary>
    /// Gets all available channels as ChannelViewModel instances for binding to a ComboBox.
    /// </summary>
    public static IEnumerable<OutputDevicesViewModel> AllOutputDevices
    {
        get
        {
            return fractions.OutputDevice.InstalledDevices
                .Select(ioc => new OutputDevicesViewModel(ioc));
        }
    }
}
