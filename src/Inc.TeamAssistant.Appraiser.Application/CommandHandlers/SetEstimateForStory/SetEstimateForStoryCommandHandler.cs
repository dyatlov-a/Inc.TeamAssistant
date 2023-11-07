using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory;

internal sealed class SetEstimateForStoryCommandHandler : IRequestHandler<SetEstimateForStoryCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessagesSender _messagesSender;

	public SetEstimateForStoryCommandHandler(
		IAssessmentSessionRepository repository,
		SummaryByStoryBuilder summaryByStoryBuilder,
		IMessagesSender messagesSender)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
		_messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
	}

    public async Task<CommandResult> Handle(SetEstimateForStoryCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.AppraiserId).EnsureForAppraiser(command.AppraiserName);

		var appraiser = assessmentSession.Participants.Single(a => a.Id == command.AppraiserId);
        assessmentSession.Estimate(appraiser, command.Value.ToAssessmentValue());
        
        await _messagesSender.StoryChanged(assessmentSession.Id);

        return CommandResult.Build(
	        await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(assessmentSession)));
    }
}