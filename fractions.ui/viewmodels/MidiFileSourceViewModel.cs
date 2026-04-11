using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fractions.ui.pipeline;
using System.IO;

namespace fractions.ui.viewmodels;

public partial class MidiFileSourceViewModel(IOutputDevice device, Clock clock) : BaseViewModel(), INoteSource
{
    private readonly IOutputDevice _device = device;
    private readonly Clock _clock = clock;

    [ObservableProperty]
    private string filePath = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";

    private bool CanLoadFile
    {
        get
        {
            return !string.IsNullOrEmpty(FilePath) && File.Exists(FilePath);
        }
    }

    [ObservableProperty]
    private MidiFile readMidiFileResult;

    [RelayCommand(CanExecute = nameof(CanLoadFile))]
    public void ReadMidiFile()
    {
        try
        {
            ReadMidiFileResult = new MidiFile(FilePath);
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [ObservableProperty]
    private IEnumerable<NoteOnOffMessage> messages;

    [RelayCommand(CanExecute = nameof(CanLoadFile))]
    public void ReadMessages()
    {
        try
        {
            Messages = ReadMidiFileResult.GetNotes(_device, _clock);
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    public IEnumerable<NoteOnOffMessage> Load()
    {
        return Messages;
    }
}