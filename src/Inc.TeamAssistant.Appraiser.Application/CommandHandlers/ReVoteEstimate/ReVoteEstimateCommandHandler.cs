using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate;

internal sealed class ReVoteEstimateCommandHandler : IRequestHandler<ReVoteEstimateCommand, ReVoteEstimateResult>
{
    private readonly IAssessmentSessionRepository _repository;

	public ReVoteEstimateCommandHandler(IAssessmentSessionRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

    public Task<ReVoteEstimateResult> Handle(ReVoteEstimateCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

		assessmentSession.Reset(command.ModeratorId);

        var estimateEnded = false;
        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(s => new EstimateItemDetails(
                s.Participant.Name,
                s.Value.ToDisplayHasValue(),
                s.Value.ToDisplayValue()))
            .ToArray();
        var result = new ReVoteEstimateResult(
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