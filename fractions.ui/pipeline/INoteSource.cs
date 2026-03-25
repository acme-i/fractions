using System.Collections.Generic;

namespace fractions.ui.pipeline;

public interface INoteSource
{
    IEnumerable<NoteOnOffMessage> Load();
}