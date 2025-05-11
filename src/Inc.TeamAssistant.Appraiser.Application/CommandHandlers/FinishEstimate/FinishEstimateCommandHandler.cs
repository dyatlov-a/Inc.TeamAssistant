using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishEstimate;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishEstimate;

internal sealed class FinishEstimateCommandHandler : IRequestHandler<FinishEstimateCommand, CommandResult>
{
    private readonly IStoryRepository _repository;
    private readonly SummaryByStoryBuilder _summaryBuilder;
    private readonly IAssessmentSessionEventSender _eventSender;
    private readonly ITeamAccessor _teamAccessor;

    public FinishEstimateCommandHandler(
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

    public async Task<CommandResult> Handle(FinishEstimateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var personId = command.MessageContext.Person.Id;
        
        var story = await command.StoryId.Required(_repository.Find, token);
        var hasManagerAccess = await _teamAccessor.HasManagerAccess(new(story.TeamId, personId), token);
        
        await _repository.Upsert(story.Finish(personId, hasManagerAccess), token);

        var notification = _summaryBuilder.Build(SummaryByStoryConverter.ConvertTo(story));
        await _eventSender.StoryChanged(story.TeamId);
        return CommandResult.Build(notification);
    }
}