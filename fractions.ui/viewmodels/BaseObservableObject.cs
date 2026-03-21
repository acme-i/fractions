using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace fractions.ui.viewmodels;

public class BaseObservableObject : ObservableObject
{
    internal Dispatcher? Dispatcher;

    public BaseObservableObject()
    {
    }

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
}
