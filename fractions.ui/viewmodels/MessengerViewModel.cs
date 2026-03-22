using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace fractions.ui.viewmodels;

public partial class MessengerViewModel : BaseObservableObject
{
    public MessengerViewModel(IMessenger messenger)
    {
        ArgumentNullException.ThrowIfNull(messenger);

        Messenger = messenger;
    }

    [ObservableProperty]
    private IMessenger messenger;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    [NotifyPropertyChangedFor(nameof(StatusMessage))]
    protected Exception? lastException;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public string ErrorMessage => LastException?.Message ?? string.Empty;
}