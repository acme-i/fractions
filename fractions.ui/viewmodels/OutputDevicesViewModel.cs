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
    public OutputDevicesViewModel()
    {
    }

    private string? _selectedOutputDevice;

    /// <summary>
    /// The selected channel value.
    /// </summary>
    public string? SelectedOutputDevice
    {
        get => _selectedOutputDevice;
        set
        {
            if (_selectedOutputDevice != value)
            {
                NotifyPropertyChangingOnUiThread(nameof(SelectedOutputDevice));
                _selectedOutputDevice = value ?? "[N/A]";
                if(fractions.OutputDevice.InstalledDevices.FirstOrDefault(ioc => ioc.Name == value) is IOutputDevice device)
                {
                    App.OutputDevice = device;
                }
                NotifyPropertyChangedOnUiThread(nameof(SelectedOutputDevice));
            }
        }
    }

    /// <summary>
    /// Gets all available channels as ChannelViewModel instances for binding to a ComboBox.
    /// </summary>
    public static IEnumerable<string> AllOutputDevices
    {
        get
        {
            return fractions.OutputDevice.InstalledDevices.Select(ioc => ioc.Name);
        }
    }
}
