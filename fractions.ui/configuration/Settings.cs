
using System;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace fractions.ui.configuration;
public partial class Settings : ObservableObject, ISettings
{
    public Settings() : base()
    {
    }

    [ObservableProperty]
    private string version = "1.0";
}
