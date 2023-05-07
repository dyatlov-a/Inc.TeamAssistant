using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
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

    public Task<SetEstimateForStoryResult> Handle(
	    SetEstimateForStoryCommand command,
	    CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.AppraiserId).EnsureForAppraiser(command.AppraiserName);

		var appraiser = assessmentSession.Participants.Single(a => a.Id == command.AppraiserId);
        assessmentSession.Estimate(appraiser, command.Value.ToAssessmentValue());

        return Task.FromResult<SetEstimateForStoryResult>(new(SummaryByStoryConverter.ConvertTo(assessmentSession)));
	}
}