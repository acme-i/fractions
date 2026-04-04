using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace fractions.ui.viewmodels;

public partial class ClockViewModel(Clock clock) : BaseViewModel()
{
    private Clock _clock = clock;

    public float BeatsPerMinute
    {
        get => _clock.BeatsPerMinute;
        set
        {
            if (_clock.IsRunning == false)
            {
                NotifyPropertyChangingOnUiThread(nameof(BeatsPerMinute));
                _clock.BeatsPerMinute = value;
                NotifyPropertyChangedOnUiThread(nameof(BeatsPerMinute));
            }
        }
    }

    public float Time => _clock.Time;
    public bool IsRunning => _clock.IsRunning;
    public bool IsNotRunning => !_clock.IsRunning;

    [RelayCommand(CanExecute = nameof(IsNotRunning))]
    public void Reset()
    {
        try
        {
            _clock.Reset();
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [RelayCommand(CanExecute = nameof(IsNotRunning))]
    public void Start()
    {
        try
        {
            if(!App.OutputDevice.IsOpen)
            {
                App.OutputDevice.Open();
            }
            _clock.Start();
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [RelayCommand(CanExecute = nameof(IsRunning))]
    public void Stop()
    {
        try
        {
            _clock.Stop();
            if (App.OutputDevice.IsOpen)
            {
                App.OutputDevice.Close();
            }
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }


    private void NotifyStateChanged()
    {
        NotifyPropertyChangedOnUiThread(nameof(IsRunning));
        NotifyPropertyChangedOnUiThread(nameof(IsNotRunning));
        NotifyPropertyChangedOnUiThread(nameof(BeatsPerMinute));
        NotifyPropertyChangedOnUiThread(nameof(Time));

        // Tell WPF to re-evaluate CanExecute on all three commands
        ResetCommand.NotifyCanExecuteChanged();
        StartCommand.NotifyCanExecuteChanged();
        StopCommand.NotifyCanExecuteChanged();
    }
}
