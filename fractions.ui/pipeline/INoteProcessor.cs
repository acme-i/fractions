using System.Collections.Generic;

namespace fractions.ui.pipeline;

public interface INoteProcessor
{
    string Name { get; }
    IEnumerable<NoteOnOffMessage> Process(IEnumerable<NoteOnOffMessage> input);
}