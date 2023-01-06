using System.Text;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ShowHelpNotificationBuilder : INotificationBuilder<ShowHelpResult>
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly string _noIdeaCommand;

    public ShowHelpNotificationBuilder(IMessageBuilder messageBuilder, string noIdeaCommand)
    {
        if (string.IsNullOrWhiteSpace(noIdeaCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(noIdeaCommand));

        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _noIdeaCommand = noIdeaCommand;
    }

    public async IAsyncEnumerable<NotificationMessage> Build(ShowHelpResult commandResult, long fromId)
    {
        if (commandResult == null)
            throw new ArgumentNullException(nameof(commandResult));

        var messageBuilder = new StringBuilder();

        foreach (var commandHelp in commandResult.CommandsHelp)
            messageBuilder.AppendLine(commandHelp);

        messageBuilder.AppendLine(await _messageBuilder.Build(
            Messages.NoIdeaHelp,
            commandResult.LanguageId,
            _noIdeaCommand));

        yield return NotificationMessage.Create(fromId, messageBuilder.ToString());
    }
}