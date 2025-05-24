using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;

internal sealed class AddStoryCommandHandler : IRequestHandler<AddStoryCommand, CommandResult>
{
    private readonly IStoryRepository _repository;
    private readonly SummaryByStoryBuilder _summaryBuilder;
    private readonly IAssessmentSessionEventSender _eventSender;
    private readonly ITeamAccessor _teamAccessor;

    public AddStoryCommandHandler(
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

    public async Task<CommandResult> Handle(AddStoryCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var teammates = await _teamAccessor.GetTeammates(command.TeamId, DateTimeOffset.UtcNow, token);
        if (!teammates.Any())
            throw new TeamAssistantException($"Teammates no found for Team {command.TeamId}.");
        
        var targetTeam = command.MessageContext.EnsureTeam(command.TeamId);
        var story = new Story(
            Guid.NewGuid(),
            command.MessageContext.Bot.Id,
            targetTeam.Id,
            Enum.Parse<StoryType>(command.StoryType),
            targetTeam.ChatId,
            command.MessageContext.Person.Id,
            command.MessageContext.LanguageId,
            command.Title);

        if(command.Links.Any())
            story.AddLink(command.Links.Single());

        foreach (var teammate in teammates)
            story.AddStoryForEstimate(new StoryForEstimate(
                Guid.NewGuid(),
                story.Id,
                teammate.Id,
                teammate.DisplayName));

        await _repository.Upsert(story, token);
        
        var notification = _summaryBuilder.Build(SummaryByStoryConverter.ConvertTo(story));
        await _eventSender.StoryChanged(story.TeamId);
        return CommandResult.Build(notification);
    }
}