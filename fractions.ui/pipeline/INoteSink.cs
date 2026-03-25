using System.Collections.Generic;

namespace fractions.ui.pipeline;

public interface INoteSink
{
    void Play(IEnumerable<NoteOnOffMessage> notes, Clock clock);
}