using CommunityToolkit.Mvvm.Messaging;

namespace fractions.ui.viewmodels;

public class IntegerEnumeratorViewModel(IMessenger messenger, Enumerate<int> source) : EnumerateViewModel<int>(messenger, source) { }