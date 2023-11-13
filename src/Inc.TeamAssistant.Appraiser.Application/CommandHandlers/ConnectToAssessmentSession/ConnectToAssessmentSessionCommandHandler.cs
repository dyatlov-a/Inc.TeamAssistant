using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectToAssessmentSession;

internal sealed class ConnectToAssessmentSessionCommandHandler
    : IRequestHandler<ConnectToAssessmentSessionCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IMessagesSender _messagesSender;
    private readonly IMessageBuilder _messageBuilder;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;

    public ConnectToAssessmentSessionCommandHandler(
        IAssessmentSessionRepository repository,
        IMessagesSender messagesSender,
        IMessageBuilder messageBuilder,
        SummaryByStoryBuilder summaryByStoryBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
    }

    public async Task<CommandResult> Handle(
        ConnectToAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var existSessionAssessmentSession = _repository.Find(command.AppraiserId);
        if (existSessionAssessmentSession is not null)
        {
            if (existSessionAssessmentSession.Participants.Any(p => p.Id == command.AppraiserId))
            {
                var messageId = existSessionAssessmentSession.Id == command.AssessmentSessionId
                    ? Messages.AppraiserConnectWithError
                    : Messages.AppraiserConnectedToOtherSession;

                throw new AppraiserUserException(messageId, command.AppraiserName, existSessionAssessmentSession.Title);
            }

            if (existSessionAssessmentSession.Moderator.Id == command.AppraiserId
                && existSessionAssessmentSession.Id != command.AssessmentSessionId)
                throw new AppraiserUserException(Messages.AppraiserConnectedToOtherSession, command.AppraiserName, existSessionAssessmentSession.Title);
        }
        
        // TODO: remove nullable check
        if (command.AssessmentSessionId is null)
            throw new ApplicationException("AssessmentSessionId is empty.");
        var assessmentSession = _repository
			.Find(command.AssessmentSessionId)
			.EnsureForAppraiser(command.AppraiserName);

		assessmentSession.Connect(command.AppraiserId, command.AppraiserName);

        var notifications = new List<NotificationMessage>(3);
        var connectedSuccessMessage = await _messageBuilder.Build(
            Messages.ConnectedSuccess,
            assessmentSession.LanguageId,
            assessmentSession.Title);
        notifications.Add(NotificationMessage.Create(command.TargetChatId, connectedSuccessMessage));

        if (command.AppraiserId != assessmentSession.Moderator.Id)
        {
            var appraiserAddedMessage = await _messageBuilder.Build(
                Messages.AppraiserAdded,
                assessmentSession.LanguageId,
                command.AppraiserName,
                assessmentSession.Title);
            notifications.Add(NotificationMessage.Create(assessmentSession.Moderator.Id.Value, appraiserAddedMessage));
        }

        if (assessmentSession.InProgress())
        {
            await _messagesSender.StoryChanged(assessmentSession.Id);
            notifications.Add(await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(assessmentSession)));
        }

        return CommandResult.Build(notifications.ToArray());
    }
}