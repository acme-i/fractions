using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace fractions.ui.viewmodels;

public class DoubleEnumeratorViewModel(Enumerate<double> source) : EnumerateViewModel<double>(source) { }
