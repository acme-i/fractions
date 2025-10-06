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

public abstract partial class BaseViewModel : ObservableObject
{
    #region Fields
    internal Dispatcher? Dispatcher;

    private readonly List<char> _unwantedChars;

    #endregion

    #region CTOR

    public BaseViewModel(IMessenger messenger, Settings settings)
    {
        ArgumentNullException.ThrowIfNull(messenger);

        Messenger = messenger;
        Settings = settings;
        statusMessage = string.Empty;
        documentTitle = string.Empty;
        _unwantedChars =
        [
            .. System.IO.Path.GetInvalidFileNameChars(),
            .. System.IO.Path.GetInvalidPathChars(),
            System.IO.Path.AltDirectorySeparatorChar,
            System.IO.Path.DirectorySeparatorChar,
        ];
    }

    #endregion CTOR

    #region Properties

    [ObservableProperty]
    private bool hasUserCancelled;

    [ObservableProperty]
    private IMessenger messenger;

    [ObservableProperty]
    private Settings settings;

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

    private string documentTitle;

    public string DocumentTitle
    {
        get
        {
            return documentTitle;
        }
        set
        {
            LastException = null;
            try
            {
                if (GetDocumentTitle(value?.Trim()) is string newTitle)
                {
                    SetProperty(ref documentTitle, newTitle, nameof(DocumentTitle));
                }
            }
            catch (Exception ex)
            {
                LastException = ex;
            }
        }
    }

    protected void UpdateStatusMessage(int updates)
    {
        StatusMessage = updates > 0 ? $"{updates} updates..." : "No updates...";
    }

    public string? GetDocumentTitle(string? documentTitle)
    {
        var title = documentTitle?.Trim();

        if (!string.IsNullOrEmpty(title))
        {
            var toRemove = _unwantedChars.ToList();
            toRemove.Add('\t');
            toRemove.Add('\n');
            toRemove.Add('\r');
            title = title.ReplaceAll(toRemove.Distinct().ToArray(), " ").Trim();
        }

        return string.IsNullOrWhiteSpace(title)
            ? null
            : title;
    }

    #endregion

    #region Commands

    #region Reload

    protected bool firstLoad = true;

    protected bool isListDirty = true;
    public static readonly Task<int> DoNothing = Task.FromResult(0);

    /// <summary>
    /// Default implementation that does nothing
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    public abstract Task<int> Reload(string? filter = null);

    #endregion Reload

    #endregion

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

    partial void OnLastExceptionChanged(Exception? value)
    {
        if (value is Exception ex)
            Trace.TraceError(ex.ToString());

        if (isVisible)
            Messenger.Send(new LastExceptionChangedMessage(value));
    }

    partial void OnStatusMessageChanged(string value)
    {
        if (isVisible)
            Messenger.Send(new StatusMessageUpdatedMessage(value));
    }

    #endregion Events

    #region Helpers

    #region Settings

    internal Exception? SaveToGlobalSettings(string key, object value)
    {
        LastException = null;
        Exception? result = default;
        try
        {
            switch (key)
            {
                case nameof(this.Settings.Version):
                    this.Settings.Version = value?.ToString() ?? "1.0";
                    break;

                default:
                    throw new ApplicationException($"Don't know how to save a {key} yet...");
            }
        }
        catch (Exception ex)
        {
            result = ex;
            LastException = ex;
        }
        return result;
    }

    #endregion Settings

    #region Notify Property Changed

    protected void NotifyPropertyChangingOnUiThread(string propertyName)
    {
        if (Dispatcher is not null && Dispatcher.CheckAccess() == false)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(NotifyPropertyChangingOnUiThread), propertyName);
            return;
        }
        OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
    }

    protected void NotifyPropertyChangedOnUiThread(string propertyName)
    {
        if (Dispatcher is not null && Dispatcher.CheckAccess() == false)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(NotifyPropertyChangedOnUiThread), propertyName);
            return;
        }
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    #endregion Notify Property Changed

    protected void ReportChanges(int inserted, string nounPlural = "updates")
    {
        if (!IsVisible) return;

        if (inserted > 0)
        {
            StatusMessage = $"{inserted} {nounPlural}...";
        }
        else
        {
            StatusMessage = $"No changes...";
        }
    }

    #endregion Helpers
}
