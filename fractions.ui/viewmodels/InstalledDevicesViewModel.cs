using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fractions.ui.viewmodels;

public partial class InstalledDevicesViewModel : BaseViewModel
{
    public InstalledDevicesViewModel(IMessenger messenger, Settings settings) : base(messenger, settings)
    {
    }
}
