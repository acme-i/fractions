using CommunityToolkit.Mvvm.Messaging.Messages;

namespace fractions.ui.messages;
public class LastExceptionChangedMessage(Exception? value) : ValueChangedMessage<Exception?>(value)
{
}
