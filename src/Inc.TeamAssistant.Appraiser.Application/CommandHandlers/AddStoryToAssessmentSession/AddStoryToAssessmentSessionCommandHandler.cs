using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.DialogContinuations;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryToAssessmentSession;

internal sealed class AddStoryToAssessmentSessionCommandHandler
    : IRequestHandler<AddStoryToAssessmentSessionCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation<ContinuationState> _dialogContinuation;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessagesSender _messagesSender;

    public AddStoryToAssessmentSessionCommandHandler(
        IAssessmentSessionRepository repository,
        IDialogContinuation<ContinuationState> dialogContinuation,
        SummaryByStoryBuilder summaryByStoryBuilder,
        IMessagesSender messagesSender)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
    }

    public async Task<CommandResult> Handle(
        AddStoryToAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

        assessmentSession.StorySelected(command.ModeratorId, command.Title.Trim(), command.Links);
        _dialogContinuation.End(command.ModeratorId, ContinuationState.EnterStory);
        
        await _messagesSender.StoryChanged(assessmentSession.Id);

        return CommandResult.Build(
            await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(assessmentSession)));
    }
}