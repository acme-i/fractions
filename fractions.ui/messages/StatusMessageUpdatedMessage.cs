using CommunityToolkit.Mvvm.Messaging.Messages;

namespace fractions.ui.messages;
public class StatusMessageUpdatedMessage(string value) : ValueChangedMessage<string>(value)
{
}
