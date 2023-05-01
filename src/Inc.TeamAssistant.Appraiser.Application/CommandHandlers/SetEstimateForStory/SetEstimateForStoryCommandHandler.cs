using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Domain;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory;

internal sealed class SetEstimateForStoryCommandHandler : IRequestHandler<SetEstimateForStoryCommand, SetEstimateForStoryResult>
{
    private readonly IAssessmentSessionRepository _repository;

	public SetEstimateForStoryCommandHandler(IAssessmentSessionRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

    public Task<SetEstimateForStoryResult> Handle(SetEstimateForStoryCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.AppraiserId).EnsureForAppraiser(command.AppraiserName);

		var appraiser = assessmentSession.Participants.Single(a => a.Id == command.AppraiserId);
        assessmentSession.Estimate(appraiser, command.Value.ToAssessmentValue());

        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(s => new EstimateItemDetails(
                s.Participant.Name,
                s.Value.ToDisplayHasValue(),
                s.Value.ToDisplayValue()))
            .ToArray();

		var estimateEnded = assessmentSession.EstimateEnded();

        var result = new SetEstimateForStoryResult(
            assessmentSession.Id,
            assessmentSession.LanguageId,
            new(
                assessmentSession.ChatId,
                StoryConverter.ConvertTo(assessmentSession.CurrentStory),
                assessmentSession.CurrentStory.GetTotal().ToDisplayValue(estimateEnded),
                items),
            estimateEnded);

		return Task.FromResult(result);
	}
}