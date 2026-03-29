using CommunityToolkit.Mvvm.Messaging;

namespace fractions.ui.viewmodels;

public class IntegerEnumeratorViewModel : EnumerateViewModel<int>
{
    public IntegerEnumeratorViewModel(IMessenger messenger) : base(messenger)
    {
    }
}