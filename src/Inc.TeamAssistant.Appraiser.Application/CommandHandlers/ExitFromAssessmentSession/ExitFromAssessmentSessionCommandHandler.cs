using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ExitFromAssessmentSession;

internal sealed class ExitFromAssessmentSessionCommandHandler
    : IRequestHandler<ExitFromAssessmentSessionCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IMessagesSender _messagesSender;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessageBuilder _messageBuilder;

	public ExitFromAssessmentSessionCommandHandler(
        IAssessmentSessionRepository repository,
        IMessagesSender messagesSender,
        SummaryByStoryBuilder summaryByStoryBuilder,
        IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
        _summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(
        ExitFromAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository
            .Find(command.AppraiserId)
            .EnsureForAppraiser(command.AppraiserName);

		assessmentSession.Disconnect(command.AppraiserId);

        var disconnectedFromSessionMessage = await _messageBuilder.Build(
            Messages.DisconnectedFromSession,
            assessmentSession.LanguageId,
            assessmentSession.Title);
        var appraiserDisconnectedFromSessionMessage = await _messageBuilder.Build(
            Messages.AppraiserDisconnectedFromSession,
            assessmentSession.LanguageId,
            command.AppraiserName,
            assessmentSession.Title);

        var notifications = new List<NotificationMessage>();
        notifications.Add(NotificationMessage.Create(command.TargetChatId, disconnectedFromSessionMessage));
        notifications.Add(NotificationMessage.Create(
            assessmentSession.Moderator.Id.Value,
            appraiserDisconnectedFromSessionMessage));

        if (assessmentSession.InProgress())
        {
            await _messagesSender.StoryChanged(assessmentSession.Id);
            notifications.Add(await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(assessmentSession)));
        }

        return CommandResult.Build(notifications.ToArray());
    }
}