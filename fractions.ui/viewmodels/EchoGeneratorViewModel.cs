using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fractions.ui.pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace fractions.ui.viewmodels;

public partial class EchoGeneratorViewModel : ObservableObject, INoteProcessor
{
    public EchoGeneratorViewModel()
    {
    }

    public EchoGeneratorViewModel(
        IntegerEnumeratorViewModel echoes,
        FloatEnumeratorViewModel offsets,
        IntegerEnumeratorViewModel pitches,
        FloatEnumeratorViewModel velocityOffsets
    )
    {
        EchoesViewModel = echoes;
        OffsetsViewModel = offsets;
        PitchesViewModel = pitches;
        VelocityOffsetsViewModel = velocityOffsets;
    }

    public string Name => "Echo Generator";

    [ObservableProperty]
    private IntegerEnumeratorViewModel echoesViewModel;

    [ObservableProperty]
    private FloatEnumeratorViewModel offsetsViewModel;

    [ObservableProperty]
    private IntegerEnumeratorViewModel pitchesViewModel;

    [ObservableProperty]
    private FloatEnumeratorViewModel velocityOffsetsViewModel;

    [ObservableProperty]
    private bool incrementEchoesPerEcho = false;

    [ObservableProperty]
    private bool incrementPitchesPerEcho = true;

    [ObservableProperty]
    private bool incrementOffsetsPerEcho = true;

    [ObservableProperty]
    private bool incrementVelocityOffsetsPerEcho = true;

    public IEnumerable<NoteOnOffMessage> Process(IEnumerable<NoteOnOffMessage> input)
    {
        var list = input.ToList();
        var output = new List<NoteOnOffMessage>(input.Count() * 10);

        foreach (var note in list)
        {
            output.Add(note);

            for (var i = 1; i <= EchoesViewModel.Current; i++)
            {
                var echo = new NoteOnOffMessage(note, PitchesViewModel.Current + i, OffsetsViewModel.Current * i);
                echo.Velocity = note.Velocity * Math.Pow(VelocityOffsetsViewModel.Current, i);
                output.Add(echo);

                if (IncrementEchoesPerEcho) EchoesViewModel.GoToNextCommand.Execute(null);
                if (IncrementOffsetsPerEcho) OffsetsViewModel.GoToNextCommand.Execute(null);
                if (IncrementPitchesPerEcho) PitchesViewModel.GoToNextCommand.Execute(null);
                if (IncrementVelocityOffsetsPerEcho) VelocityOffsetsViewModel.GoToNextCommand.Execute(null);
            }

            if (!IncrementEchoesPerEcho) EchoesViewModel.GoToNextCommand.Execute(null);
            if (!IncrementOffsetsPerEcho) OffsetsViewModel.GoToNextCommand.Execute(null);
            if (!IncrementPitchesPerEcho) PitchesViewModel.GoToNextCommand.Execute(null);
            if (!IncrementVelocityOffsetsPerEcho) VelocityOffsetsViewModel.GoToNextCommand.Execute(null);
        }

        return output;
    }

}