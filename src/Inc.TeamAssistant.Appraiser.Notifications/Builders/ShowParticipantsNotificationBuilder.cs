using System.Text;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ShowParticipantsNotificationBuilder : INotificationBuilder<ShowParticipantsResult>
{
	private readonly IMessageBuilder _messageBuilder;

	public ShowParticipantsNotificationBuilder(IMessageBuilder messageBuilder)
	{
		_messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
	}

	public async IAsyncEnumerable<NotificationMessage> Build(ShowParticipantsResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		var messageBuilder = new StringBuilder();

		messageBuilder.AppendLine(await _messageBuilder.Build(
            Messages.AppraiserList,
            commandResult.AssessmentSessionLanguageId));
		foreach (var appraiser in commandResult.Appraisers)
			messageBuilder.AppendLine(appraiser);

        yield return NotificationMessage.Create(fromId, messageBuilder.ToString());
	}
}