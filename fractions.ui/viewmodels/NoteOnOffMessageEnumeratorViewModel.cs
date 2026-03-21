using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;

namespace fractions.ui.viewmodels;

public class NoteOnOffMessageEnumeratorViewModel(Enumerate<NoteOnOffMessage> source) : EnumerateViewModel<NoteOnOffMessage>(source)
{
}