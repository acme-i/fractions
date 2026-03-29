using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;

namespace fractions.ui.viewmodels;

public class FloatEnumeratorViewModel(IMessenger messenger) : EnumerateViewModel<float>(messenger)
{
}
