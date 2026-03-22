using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;

namespace fractions.ui.viewmodels;

public class FloatEnumeratorViewModel(IMessenger messenger, Enumerate<float> source) : EnumerateViewModel<float>(messenger, source)
{
}
