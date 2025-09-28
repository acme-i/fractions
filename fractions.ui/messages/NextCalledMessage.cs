using CommunityToolkit.Mvvm.Messaging.Messages;

namespace fractions.ui.messages;

public class NextCalledMessage<T>(Enumerate<T> value) : ValueChangedMessage<Enumerate<T>>(value)
{
}
