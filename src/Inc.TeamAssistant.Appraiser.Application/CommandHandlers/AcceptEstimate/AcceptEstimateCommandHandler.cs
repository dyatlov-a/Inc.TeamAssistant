using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate;

internal sealed class AcceptEstimateCommandHandler : IRequestHandler<AcceptEstimateCommand, AcceptEstimateResult>
{
    private readonly IAssessmentSessionRepository _repository;

	public AcceptEstimateCommandHandler(IAssessmentSessionRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

	public Task<AcceptEstimateResult> Handle(AcceptEstimateCommand command, CancellationToken cancellationToken)
	{
		if (command is null)
			throw new ArgumentNullException(nameof(command));

		var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

		assessmentSession.CompleteEstimate(command.ModeratorId);

		var items = assessmentSession.CurrentStory.StoryForEstimates
			.Select(s => new EstimateItemDetails(
			    s.Participant.Id,
			    s.Participant.Name,
			    s.StoryExternalId,
			    s.Value.ToDisplayHasValue(),
			    s.Value.ToDisplayValue()))
			.ToArray();

        var result = new AcceptEstimateResult(
            assessmentSession.Id,
            assessmentSession.LanguageId,
            new(
                assessmentSession.ChatId,
                StoryConverter.ConvertTo(assessmentSession.CurrentStory),
                assessmentSession.CurrentStory.GetTotal().ToDisplayValue(estimateEnded: true),
                items));

        return Task.FromResult(result);
    }
}