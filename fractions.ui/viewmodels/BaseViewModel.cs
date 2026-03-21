using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using fractions.ui.messages;

namespace fractions.ui.viewmodels;

public partial class BaseViewModel : BaseObservableObject
{
    public BaseViewModel(IMessenger messenger, Settings settings)
    {
        ArgumentNullException.ThrowIfNull(messenger);

        Messenger = messenger;
        statusMessage = string.Empty;
    }

    [ObservableProperty]
    private bool hasUserCancelled;

    [ObservableProperty]
    private IMessenger messenger;

    [ObservableProperty]
    protected bool isLoaded;

    [ObservableProperty]
    protected bool isVisible;

    [ObservableProperty]
    protected string cancelCommandText = "Cancel";

    public string ErrorMessage => LastException?.Message ?? string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    [NotifyPropertyChangedFor(nameof(StatusMessage))]
    protected Exception? lastException;

    [ObservableProperty]
    private string statusMessage;

    protected bool firstLoad = true;

    protected bool isListDirty = true;
    public static readonly Task<int> DoNothing = Task.FromResult(0);

    [RelayCommand]
    public Task<int> Reload(string? filter = null)
    {
        return DoNothing;
    }

    [ObservableProperty]
    private Settings settings;

    protected void ReportChanges(int inserted, string nounPlural = "updates")
    {
        if (inserted > 0)
        {
            StatusMessage = $"{inserted} {nounPlural}...";
        }
        else
        {
            StatusMessage = $"No changes...";
        }
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

    protected virtual async Task OnIsVisibleChangedInternal(bool value)
    {
        if (firstLoad || value)
        {
            await Reload(null).ConfigureAwait(false);
        }
    }

    partial void OnIsVisibleChanged(bool value)
    {
        Task.Run(() => OnIsVisibleChangedInternal(value));
    }

    #endregion Events
}
