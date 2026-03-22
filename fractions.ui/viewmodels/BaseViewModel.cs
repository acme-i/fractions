using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using fractions.ui.messages;

namespace fractions.ui.viewmodels;

public partial class BaseViewModel : MessengerViewModel
{
    public BaseViewModel(IMessenger messenger, Settings settings) : base(messenger)
    {
        ArgumentNullException.ThrowIfNull(messenger);
        Settings = settings;
    }

    [ObservableProperty]
    private bool hasUserCancelled;

    [ObservableProperty]
    protected bool isLoaded;

    [ObservableProperty]
    protected bool isVisible;

    [ObservableProperty]
    protected string cancelCommandText = "Cancel";

    [ObservableProperty]
    private Settings settings;

    [RelayCommand]
    public Task Cancel()
    {
        return Task.CompletedTask;
    }

    #region Settings

    internal Exception? SaveToGlobalSettings(string key, object value)
    {
        LastException = null;
        Exception? result = default;
        try
        {
            this.Settings.Version = key switch
            {
                nameof(this.Settings.Version) => value?.ToString() ?? "1.0",
                _ => throw new ApplicationException($"Don't know how to save a {key} yet..."),
            };
        }
        catch (Exception ex)
        {
            result = ex;
            LastException = ex;
        }
        return result;
    }

    #endregion Settings

    #region CommunityToolkit Events

    protected bool firstLoad = true;

    protected virtual async Task OnIsVisibleChangedInternal(bool value)
    {
        if (firstLoad || value)
        {
            firstLoad = false;
        }
    }

    partial void OnIsVisibleChanged(bool value)
    {
        Task.Run(() => OnIsVisibleChangedInternal(value));
    }

    #endregion Events
}
