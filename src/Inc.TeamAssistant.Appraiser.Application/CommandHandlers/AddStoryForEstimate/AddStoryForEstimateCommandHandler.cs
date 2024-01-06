using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate;

internal sealed class AddStoryForEstimateCommandHandler : IRequestHandler<AddStoryForEstimateCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;

	public AddStoryForEstimateCommandHandler(IAssessmentSessionRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

    public Task<CommandResult> Handle(AddStoryForEstimateCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

		var assessmentSession = _repository
			.Find(command.AssessmentSessionId)
			.EnsureForModerator(command.ModeratorName);

		assessmentSession.CurrentStory.SetExternalId(command.StoryExternalId);

        return Task.FromResult(CommandResult.Empty);
    }
}