using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ConnectToAssessmentSessionNotificationBuilder
    : INotificationBuilder<ConnectToAssessmentSessionResult>
{
    private readonly ICommandProvider _commandProvider;
    private readonly IMessageBuilder _messageBuilder;

    public ConnectToAssessmentSessionNotificationBuilder(ICommandProvider commandProvider, IMessageBuilder messageBuilder)
    {
        _commandProvider = commandProvider ?? throw new ArgumentNullException(nameof(commandProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async IAsyncEnumerable<NotificationMessage> Build(
	    ConnectToAssessmentSessionResult commandToAssessmentSessionResult,
	    long fromId)
	{
		if (commandToAssessmentSessionResult is null)
			throw new ArgumentNullException(nameof(commandToAssessmentSessionResult));

        var allowUseUsernameCommand = _commandProvider.GetCommand(typeof(AllowUseNameCommand));
        var connectedSuccessMessage = await _messageBuilder.Build(
            Messages.ConnectedSuccess,
            commandToAssessmentSessionResult.AssessmentSessionLanguageId,
            commandToAssessmentSessionResult.AssessmentSessionTitle,
            allowUseUsernameCommand);
        yield return NotificationMessage.Create(fromId, connectedSuccessMessage);

        if (commandToAssessmentSessionResult.ModeratorId != commandToAssessmentSessionResult.AppraiserId)
        {
	        var appraiserAddedMessage = await _messageBuilder.Build(
		        Messages.AppraiserAdded,
		        commandToAssessmentSessionResult.AssessmentSessionLanguageId,
		        commandToAssessmentSessionResult.AppraiserName,
		        commandToAssessmentSessionResult.AssessmentSessionTitle);
	        yield return NotificationMessage.Create(
		        commandToAssessmentSessionResult.ModeratorId.Value,
		        appraiserAddedMessage);
        }
        
        if (commandToAssessmentSessionResult.StoryInProgress)
        {
            var loadingMessage = await _messageBuilder.Build(Messages.Loading, commandToAssessmentSessionResult.AssessmentSessionLanguageId);

            yield return NotificationMessage
                .Create(fromId, loadingMessage)
                .AddHandler((cId, uName, mId) => AddStoryForEstimate(commandToAssessmentSessionResult.AssessmentSessionId, cId, uName, mId));
        }
    }

	private IBaseRequest AddStoryForEstimate(
        AssessmentSessionId assessmentSessionId,
		long chatId,
		string userName,
		int messageId)
	{
		if (string.IsNullOrWhiteSpace(userName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

		return new AddStoryForEstimateCommand(
			assessmentSessionId,
			chatId,
			userName,
			messageId,
			IsUpdate: true);
	}
}