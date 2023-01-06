using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ChangeLanguageNotificationBuilder : INotificationBuilder<ChangeLanguageResult>
{
    private readonly IMessageBuilder _messageBuilder;

    public ChangeLanguageNotificationBuilder(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async IAsyncEnumerable<NotificationMessage> Build(ChangeLanguageResult commandResult, long fromId)
    {
        if (commandResult == null)
            throw new ArgumentNullException(nameof(commandResult));

        var message = await _messageBuilder.Build(
            Messages.LanguageChanged,
            commandResult.SelectedLanguageId,
            commandResult.SelectedLanguageId.Value);

        yield return NotificationMessage.Create(fromId, message);
    }
}