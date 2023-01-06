using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryToAssessmentSession;

internal sealed class AddStoryToAssessmentSessionCommandHandler
    : IRequestHandler<AddStoryToAssessmentSessionCommand, AddStoryToAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation _dialogContinuation;

    public AddStoryToAssessmentSessionCommandHandler(
        IAssessmentSessionRepository repository,
        IDialogContinuation dialogContinuation)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public Task<AddStoryToAssessmentSessionResult> Handle(
        AddStoryToAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

        assessmentSession.StorySelected(command.ModeratorId, command.Title.Trim(), command.Links);
        _dialogContinuation.End(command.ModeratorId, ContinuationState.EnterStory);

        var items = assessmentSession.Participants
            .Select(a => new EstimateItemDetails(
                a.Id,
                a.Name,
                0,
                AssessmentValue.Value.None.ToDisplayHasValue(),
                AssessmentValue.Value.None.ToDisplayValue()))
            .ToArray();
        var result = new AddStoryToAssessmentSessionResult(
            assessmentSession.Id,
            assessmentSession.LanguageId,
            StoryConverter.ConvertTo(assessmentSession.CurrentStory),
            items);

        return Task.FromResult(result);
    }
}