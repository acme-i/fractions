using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;

namespace fractions.ui.viewmodels;

public class FloatEnumeratorViewModel(Enumerate<float> source) : EnumerateViewModel<float>(source) { }
