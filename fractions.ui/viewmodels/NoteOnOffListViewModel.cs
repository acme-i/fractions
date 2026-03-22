using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;

namespace fractions.ui.viewmodels;

public class NoteOnOffListViewModel(IMessenger messenger, Enumerate<NoteOnOffMessage> source)
    : EnumerateViewModel<NoteOnOffMessage>(messenger, source)
{
    public List<NoteOnOffViewModel> AllNotes => Source.Select(n => new NoteOnOffViewModel(messenger, n)).ToList();
}