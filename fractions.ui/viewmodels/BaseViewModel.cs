using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace fractions.ui.viewmodels;

public partial class BaseViewModel : BaseObservableObject
{
    [ObservableProperty]
    private bool hasUserCancelled;

    [ObservableProperty]
    protected bool isLoaded;

    [ObservableProperty]
    protected bool isVisible;

    [ObservableProperty]
    protected string cancelCommandText = "Cancel";

    [RelayCommand]
#pragma warning disable CA1822 // Mark members as static
    public Task Cancel()
#pragma warning restore CA1822 // Mark members as static
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
            App.Settings.Version = key switch
            {
                nameof(App.Settings.Version) => value?.ToString() ?? "1.0",
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
}
