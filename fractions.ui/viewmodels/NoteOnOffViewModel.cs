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

public partial class NoteOnOffViewModel : BaseViewModel
{
    public NoteOnOffViewModel(NoteOnOffMessage note) : base()
    {
        _note = note;
    }

    private NoteOnOffMessage _note;

    public float Time
    {
        get => _note.Time;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Time));
            _note.Time = value;
            NotifyPropertyChangedOnUiThread(nameof(Time));
        }
    }

    public int Channel
    {
        get => (int)_note.Channel;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Channel));
            _note.Channel = (Channel)value;
            NotifyPropertyChangedOnUiThread(nameof(Channel));
        }
    }

    public int Pitch
    {
        get => (int)_note.Pitch;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Pitch));
            _note.Pitch = (Pitch)value;
            NotifyPropertyChangedOnUiThread(nameof(Pitch));
        }
    }

    public double Velocity
    {
        get => _note.Velocity;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Velocity));
            _note.Velocity = value;
            NotifyPropertyChangedOnUiThread(nameof(Velocity));
        }
    }

    public double Pan
    {
        get => _note.Pan;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Pan));
            _note.Pan = value;
            NotifyPropertyChangedOnUiThread(nameof(Pan));
        }
    }

    public double? Reverb
    {
        get => _note.Reverb;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Reverb));
            _note.Reverb = value;
            NotifyPropertyChangedOnUiThread(nameof(Reverb));
        }
    }

    public Instrument? Instrument
    {
        get => _note.Instrument;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Instrument));
            _note.Instrument = value;
            NotifyPropertyChangedOnUiThread(nameof(Instrument));
        }
    }

    public float Duration
    {
        get => _note.Duration;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(Duration));
            _note.Duration = value;
            NotifyPropertyChangedOnUiThread(nameof(Duration));
        }
    }

    public int MinOctave
    {
        get => _note.MinOctave;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(MinOctave));
            _note.MinOctave = value;
            NotifyPropertyChangedOnUiThread(nameof(MinOctave));
        }
    }

    public int MaxOctave
    {
        get => _note.MaxOctave;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(MaxOctave));
            _note.MaxOctave = value;
            NotifyPropertyChangedOnUiThread(nameof(MaxOctave));
        }
    }

    [RelayCommand]
    public void SendNow()
    {
        _note.SendNow();
    }
}
