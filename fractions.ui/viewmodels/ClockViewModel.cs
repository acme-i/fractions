using CommunityToolkit.Mvvm.ComponentModel;
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
    private readonly Clock _clock = clock;

    public float BeatsPerMinute
    {
        get => _clock.BeatsPerMinute;
        set
        {
            if (_clock.IsRunning == false && _clock.BeatsPerMinute != value)
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
    public void SetBeatsPerMinute(float value)
    {
        try
        {
            this.BeatsPerMinute = value;
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [RelayCommand(CanExecute = nameof(IsNotRunning))]
    public void Reset()
    {
        try
        {
            if (_clock.IsRunning)
            {
                _clock.Stop();
            }
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
            if (!App.OutputDevice.IsOpen)
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
            if (_clock.IsRunning)
            {
                _clock.Stop();
            }
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

        // Tell WPF to re-evaluate CanExecute on all four commands
        ResetCommand.NotifyCanExecuteChanged();
        StartCommand.NotifyCanExecuteChanged();
        StopCommand.NotifyCanExecuteChanged();
        SetBeatsPerMinuteCommand.NotifyCanExecuteChanged();
    }
}
