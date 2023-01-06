using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate;

internal sealed class AddStoryForEstimateCommandHandler
    : IRequestHandler<AddStoryForEstimateCommand, AddStoryForEstimateResult>
{
    private readonly IAssessmentSessionRepository _repository;

	public AddStoryForEstimateCommandHandler(IAssessmentSessionRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

    public Task<AddStoryForEstimateResult> Handle(
		AddStoryForEstimateCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

		var assessmentSession = _repository
			.Find(command.AssessmentSessionId)
			.EnsureForAppraiser(command.AppraiserName);

		var appraiser = assessmentSession.Participants.SingleOrDefault(a => a.Id.Value == command.TargetChatId);
        if (appraiser is null)
            throw new ApplicationException($"Participant was not found by ChatId {command.TargetChatId}");

        assessmentSession.AddStoryForEstimate(new(appraiser, command.StoryExternalId));

        var result = new AddStoryForEstimateResult(
            AssessmentSessionConverter.ConvertTo(assessmentSession),
            command.IsUpdate);

        return Task.FromResult(result);
    }
}