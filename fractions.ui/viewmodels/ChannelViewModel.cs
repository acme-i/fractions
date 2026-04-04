using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;

namespace fractions.ui.viewmodels;

/// <summary>
/// ViewModel for displaying and selecting MIDI channel values.
/// </summary>
public partial class ChannelViewModel : BaseViewModel
{
    public ChannelViewModel(Channel channel) : base()
    {
        ArgumentOutOfRangeException.ThrowIfLessThan((int)channel, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan((int)channel, 16);

        _channel = channel;
        _typeOfChannel = ChannelType.All;
    }

    private Channel _channel;

    /// <summary>
    /// The selected channel value.
    /// </summary>
    public Channel Channel
    {
        get => _channel;
        set
        {
            if (_channel != value)
            {
                NotifyPropertyChangingOnUiThread(nameof(Channel));
                NotifyPropertyChangingOnUiThread(nameof(DisplayName));
                _channel = value;
                NotifyPropertyChangedOnUiThread(nameof(Channel));
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
            return _channel.ToString();
        }
    }

    public ChannelType _typeOfChannel;

    public ChannelType TypeOfChannel
    {
        get => _typeOfChannel;
        set
        {
            NotifyPropertyChangingOnUiThread(nameof(TypeOfChannel));
            NotifyPropertyChangingOnUiThread(nameof(Channels));
            _typeOfChannel = value;
            NotifyPropertyChangedOnUiThread(nameof(TypeOfChannel));
            NotifyPropertyChangedOnUiThread(nameof(Channels));
        }
    }

    public IEnumerable<ChannelViewModel> Channels
    {
        get
        {
            return _typeOfChannel switch
            {
                ChannelType.Instruments => fractions.Channels.InstrumentChannels.Select(c => new ChannelViewModel(c)),
                ChannelType.Percussion => fractions.Channels.PercussionChannels.Select(c => new ChannelViewModel(c)),
                _ => fractions.Channels.All.Select(c => new ChannelViewModel(c)),
            };
        }
    }
}