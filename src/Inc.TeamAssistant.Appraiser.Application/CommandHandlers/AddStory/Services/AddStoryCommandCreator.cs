using System.Text;
using FluentValidation;
using FluentValidation.Results;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;

internal sealed class AddStoryCommandCreator : ICommandCreator
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly AddStoryToAssessmentSessionOptions _options;
    
    public string Command => "/add";

    public AddStoryCommandCreator(
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        AddStoryToAssessmentSessionOptions options)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
    
    public async Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext? teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext.Text.StartsWith("/"))
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    "Command",
                    await _messageBuilder.Build(Messages.Appraiser_StoryTitleIsEmpty, messageContext.LanguageId))
            });
        if (teamContext is null)
            throw new ApplicationException("Team was not selected.");
        
        var teammates = await _teamAccessor.GetTeammates(teamContext.TeamId, token);
        var storyItems = messageContext.Text.Split(' ');
        var storyTitleBuilder = new StringBuilder();
        var links = new List<string>();

        foreach (var storyItem in storyItems)
        {
            if (_options.LinksPrefix.Any(l => storyItem.StartsWith(l, StringComparison.InvariantCultureIgnoreCase)))
                links.Add(storyItem.ToLower().Trim());
            else
                storyTitleBuilder.Append($"{storyItem} ");
        }
            
        return new AddStoryCommand(
            messageContext,
            teamContext.TeamId,
            teamContext.Properties.GetValueOrDefault("StoryType", StoryType.Scrum.ToString()),
            storyTitleBuilder.ToString(),
            links,
            teammates);
    }
}