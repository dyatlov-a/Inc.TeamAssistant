using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishAssessmentSession;

internal sealed class FinishAssessmentSessionCommandHandler
    : IRequestHandler<FinishAssessmentSessionCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IAssessmentSessionMetrics _metrics;
    private readonly IMessagesSender _messagesSender;
    private readonly IMessageBuilder _messageBuilder;

	public FinishAssessmentSessionCommandHandler(
        IAssessmentSessionRepository repository,
        IAssessmentSessionMetrics metrics,
        IMessagesSender messagesSender,
        IMessageBuilder messageBuilder)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(
        FinishAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

		if (assessmentSession.Moderator.Id != command.ModeratorId)
			throw new AppraiserUserException(Messages.EndAssessmentWithError);

        _repository.Remove(assessmentSession);
        _metrics.Ended();
        
        await _messagesSender.StoryChanged(assessmentSession.Id);

        var targets = assessmentSession.Participants
            .Select(a => a.Id)
            .Append(command.TargetChatId)
            .Distinct()
            .ToArray();
        var message = await _messageBuilder.Build(
            Messages.SessionEnded,
            assessmentSession.LanguageId,
            assessmentSession.Title);

        return CommandResult.Build(NotificationMessage.Create(targets, message));
    }
}