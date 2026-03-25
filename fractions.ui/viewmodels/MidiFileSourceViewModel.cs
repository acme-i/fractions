using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.pipeline;
using System.Collections.Generic;

namespace fractions.ui.viewmodels;

public partial class MidiFileSourceViewModel : MessengerViewModel, INoteSource
{
    private readonly IOutputDevice _device;
    private readonly Clock _clock;

    public MidiFileSourceViewModel(IMessenger messenger, IOutputDevice device, Clock clock)
        : base(messenger)
    {
        _device = device;
        _clock = clock;
    }

    [ObservableProperty]
    private string filePath = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";

    public IEnumerable<NoteOnOffMessage> Load()
    {
        if (_device == null)
            throw new InvalidOperationException("No output device available.");

        var file = new MidiFile(FilePath);
        return file.GetNotes(_device, _clock);
    }
}