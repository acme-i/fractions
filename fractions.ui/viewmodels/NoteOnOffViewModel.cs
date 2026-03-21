using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fractions.ui.viewmodels;

public partial class NoteOnOffViewModel : BaseObservableObject
{
    public NoteOnOffViewModel(IOutputDevice device, Clock clock) : base()
    {
        _source = new NoteOnOffMessage(device, fractions.Channel.Channel1, fractions.Pitch.C4, 120, 0, clock, 4);
    }

    private NoteOnOffMessage _source;

    public int Channel
    {
        get => (int)_source.Channel;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Channel));
            _source.Channel = (Channel)value;
            NotifyPropertyChangedOnUiThread(nameof(Channel));
        }
    }

    public int Pitch
    {
        get => (int)_source.Pitch;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Pitch));
            _source.Pitch = (Pitch)value;
            NotifyPropertyChangedOnUiThread(nameof(Pitch));
        }
    }

    public double Velocity
    {
        get => _source.Velocity;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Velocity));
            _source.Velocity = value;
            NotifyPropertyChangedOnUiThread(nameof(Velocity));
        }
    }

    public double Pan
    {
        get => _source.Pan;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Pan));
            _source.Pan = value;
            NotifyPropertyChangedOnUiThread(nameof(Pan));
        }
    }

    public double? Reverb
    {
        get => _source.Reverb;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Reverb));
            _source.Reverb = value;
            NotifyPropertyChangedOnUiThread(nameof(Reverb));
        }
    }

    public Instrument? Instrument
    {
        get => _source.Instrument;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Instrument));
            _source.Instrument = value;
            NotifyPropertyChangedOnUiThread(nameof(Instrument));
        }
    }

    public float Duration
    {
        get => _source.Duration;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Duration));
            _source.Duration = value;
            NotifyPropertyChangedOnUiThread(nameof(Duration));
        }
    }

    public int MinOctave
    {
        get => _source.MinOctave;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(MinOctave));
            _source.MinOctave = value;
            NotifyPropertyChangedOnUiThread(nameof(MinOctave));
        }
    }

    public int MaxOctave
    {
        get => _source.MaxOctave;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(MaxOctave));
            _source.MaxOctave = value;
            NotifyPropertyChangedOnUiThread(nameof(MaxOctave));
        }
    }

    [RelayCommand]
    public void SendNow()
    {
        _source.SendNow();
    }
}
