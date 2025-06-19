using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate;

internal sealed class ReVoteEstimateCommandHandler : IRequestHandler<ReVoteEstimateCommand, CommandResult>
{
    private readonly IStoryRepository _repository;
    private readonly SummaryByStoryBuilder _summaryBuilder;
    private readonly IAssessmentSessionEventSender _eventSender;
    private readonly ITeamAccessor _teamAccessor;

	public ReVoteEstimateCommandHandler(
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

    public async Task<CommandResult> Handle(ReVoteEstimateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var personId = command.MessageContext.Person.Id;
        
        var story = await command.StoryId.Required(_repository.Find, token);
        var hasManagerAccess = await _teamAccessor.HasManagerAccess(new(story.TeamId, personId), token);
        
        await _repository.Upsert(story.Reset(personId, hasManagerAccess), token);

        var notification = _summaryBuilder.Build(SummaryByStoryConverter.ConvertTo(story));
        await _eventSender.StoryChanged(story.TeamId);
        return CommandResult.Build(notification);
    }
}