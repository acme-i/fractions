using CommunityToolkit.Mvvm.ComponentModel;
using fractions.ui.pipeline;

namespace fractions.ui.viewmodels;

public partial class MidiFileSourceViewModel(IOutputDevice device, Clock clock) : BaseViewModel(), INoteSource
{
    private readonly IOutputDevice _device = device;
    private readonly Clock _clock = clock;
    [ObservableProperty]
    private string filePath = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";

    public IEnumerable<NoteOnOffMessage> Load()
    {
        if(_device==null)
        {
            LastException = new InvalidOperationException("No output device available.");
            return new List<NoteOnOffMessage>();
        }

        if (_device.IsOpen == false)
        {
            _device.Open();
        }

        var file = new MidiFile(FilePath);
        return file.GetNotes(_device, _clock);
    }
}