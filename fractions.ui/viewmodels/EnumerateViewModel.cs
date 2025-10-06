#region Usings
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using fractions.ui.messages;

namespace fractions.ui.viewmodels;
#endregion

public class EnumerateViewModel<T>(IMessenger messenger, Settings settings) : BaseViewModel(messenger, settings)
{
    public override Task<int> Reload(string? filter = null)
    {
        return Task.FromResult(0);
    }
}
