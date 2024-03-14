using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;

internal sealed class AddStoryCommandCreator : ICommandCreator
{
    private readonly ITeamAccessor _teamAccessor;
    private readonly AppraiserOptions _options;
    
    public string Command => CommandList.AddStory;

    public AddStoryCommandCreator(ITeamAccessor teamAccessor, AppraiserOptions options)
    {
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
    
    public async Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));

        var separator = ' ';
        var teammates = await _teamAccessor.GetTeammates(teamContext.TeamId, token);
        var storyItems = messageContext.Text.Split(separator);
        var links = new List<string>();
        var text = new List<string>();

        foreach (var storyItem in storyItems)
        {
            if (_options.LinksPrefix.Any(l => storyItem.StartsWith(l, StringComparison.InvariantCultureIgnoreCase)))
                links.Add(storyItem.ToLower());
            else
                text.Add(storyItem);
        }
            
        return new AddStoryCommand(
            messageContext,
            teamContext.TeamId,
            teamContext.Properties.GetValueOrDefault("storyType", StoryType.Scrum.ToString()),
            string.Join(separator, text),
            links,
            teammates);
    }
}