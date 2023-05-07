using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;

internal sealed class StartStorySelectionCommandHandler
    : IRequestHandler<StartStorySelectionCommand, StartStorySelectionResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation _dialogContinuation;

    public StartStorySelectionCommandHandler(
        IAssessmentSessionRepository repository,
        IDialogContinuation dialogContinuation)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public Task<StartStorySelectionResult> Handle(
        StartStorySelectionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

		assessmentSession.StartStorySelection(command.ModeratorId);
        _dialogContinuation.Begin(command.ModeratorId, ContinuationState.EnterStory);

        return Task.FromResult<StartStorySelectionResult>(new(assessmentSession.LanguageId.Value));
    }
}