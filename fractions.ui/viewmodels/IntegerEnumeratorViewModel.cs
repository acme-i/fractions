using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;

namespace fractions.ui.viewmodels;

public class IntegerEnumeratorViewModel(Enumerate<int> source) : EnumerateViewModel<int>(source)
{
}
