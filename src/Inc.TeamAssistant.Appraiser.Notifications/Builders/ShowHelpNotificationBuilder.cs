using System.Text;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ShowHelpNotificationBuilder : INotificationBuilder<ShowHelpResult>
{
    public async IAsyncEnumerable<NotificationMessage> Build(ShowHelpResult commandResult, long fromId)
    {
        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        var messageBuilder = new StringBuilder();

        foreach (var commandHelp in commandResult.CommandsHelp)
            messageBuilder.AppendLine(commandHelp);

        yield return NotificationMessage.Create(fromId, messageBuilder.ToString());

        await Task.Yield();
    }
}