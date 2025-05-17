using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate;

internal sealed class AcceptEstimateCommandHandler : IRequestHandler<AcceptEstimateCommand, CommandResult>
{
	private readonly IStoryRepository _repository;
    private readonly SummaryByStoryBuilder _summaryBuilder;
    private readonly IAssessmentSessionEventSender _eventSender;
    private readonly ITeamAccessor _teamAccessor;

	public AcceptEstimateCommandHandler(
		IStoryRepository repository,
		SummaryByStoryBuilder summaryBuilder,
		IAssessmentSessionEventSender eventSender,
		ITeamAccessor teamAccessor)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_summaryBuilder = summaryBuilder ?? throw new ArgumentNullException(nameof(summaryBuilder));
		_eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
		_teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
	}

	public async Task<CommandResult> Handle(AcceptEstimateCommand command, CancellationToken token)
	{
        ArgumentNullException.ThrowIfNull(command);

        var personId = command.MessageContext.Person.Id;
        
        var story = await command.StoryId.Required(_repository.Find, token);
		var hasManagerAccess = await _teamAccessor.HasManagerAccess(new(story.TeamId, personId), token);
		var estimation = story.Accept(personId, hasManagerAccess, command.Value);

		await _repository.Upsert(story, token);

		var notification = _summaryBuilder.Build(SummaryByStoryConverter.ConvertTo(story));
		await _eventSender.StoryAccepted(story.TeamId, estimation.DisplayValue);
		return CommandResult.Build(notification);
    }
}