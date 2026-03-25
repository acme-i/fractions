using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.pipeline;
using System.Collections.ObjectModel;
using System.Linq;

namespace fractions.ui.viewmodels;

public partial class PipelineViewModel : MessengerViewModel
{
    public PipelineViewModel(IMessenger messenger, MidiFileSourceViewModel source)
        : base(messenger)
    {
        Source = source;
    }

    public MidiFileSourceViewModel Source { get; }

    public ObservableCollection<INoteProcessor> Processors { get; } = new();

    public ObservableCollection<NoteOnOffMessage> OutputNotes { get; } = new();

    [RelayCommand]
    public void AddEcho()
    {
        Processors.Add(new EchoGeneratorViewModel());
    }

    [RelayCommand]
    public void MoveUp(INoteProcessor processor)
    {
        var idx = Processors.IndexOf(processor);
        if (idx > 0)
            Processors.Move(idx, idx - 1);
    }

    [RelayCommand]
    public void MoveDown(INoteProcessor processor)
    {
        var idx = Processors.IndexOf(processor);
        if (idx >= 0 && idx < Processors.Count - 1)
            Processors.Move(idx, idx + 1);
    }

    [RelayCommand]
    public void RunPipeline()
    {
        var notes = Source.Load();

        foreach (var p in Processors)
            notes = p.Process(notes);

        OutputNotes.Clear();
        foreach (var n in notes)
            OutputNotes.Add(n);
    }
}