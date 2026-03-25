using CommunityToolkit.Mvvm.ComponentModel;
using fractions.ui.pipeline;
using System.Collections.Generic;
using System.Linq;

namespace fractions.ui.viewmodels;

public partial class EchoGeneratorViewModel : ObservableObject, INoteProcessor
{
    public string Name => "Echo Generator";

    [ObservableProperty]
    private int echoCount = 2;

    [ObservableProperty]
    private float timeOffset = 0.15f;

    [ObservableProperty]
    private int pitchOffset = 1;

    [ObservableProperty]
    private double velocityScale = 0.85;

    public IEnumerable<NoteOnOffMessage> Process(IEnumerable<NoteOnOffMessage> input)
    {
        var list = input.ToList();
        var output = new List<NoteOnOffMessage>(list.Count * (EchoCount + 1));

        foreach (var note in list)
        {
            output.Add(note);

            for (var i = 1; i <= EchoCount; i++)
            {
                var echo = new NoteOnOffMessage(note, PitchOffset + i, TimeOffset * i);
                echo.Velocity = note.Velocity * Math.Pow(VelocityScale, i);
                output.Add(echo);
            }
        }

        return output;
    }
}