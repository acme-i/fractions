using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace fractions.ui.viewmodels;

public partial class BaseObservableObject : ObservableObject
{
    internal FrameworkElement? View;

    public BaseObservableObject()
    {
    }

    #region Notify Property Changed

    protected void NotifyPropertyChangingOnUiThread(string propertyName)
    {
        if (View?.Dispatcher is not null && View?.Dispatcher.CheckAccess() == false)
        {
            View?.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(NotifyPropertyChangingOnUiThread), propertyName);
            return;
        }
        OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
    }

    protected void NotifyPropertyChangedOnUiThread(string propertyName)
    {
        if (View?.Dispatcher is not null && View?.Dispatcher.CheckAccess() == false)
        {
            View?.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(NotifyPropertyChangedOnUiThread), propertyName);
            return;
        }
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    #endregion Notify Property Changed

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    protected Exception? lastException;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public string ErrorMessage => LastException?.Message ?? string.Empty;
}
