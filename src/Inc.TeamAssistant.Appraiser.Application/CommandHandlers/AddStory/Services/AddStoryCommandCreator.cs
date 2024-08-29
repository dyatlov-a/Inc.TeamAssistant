using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;

internal sealed class AddStoryCommandCreator : ICommandCreator
{
    private readonly ITeamAccessor _teamAccessor;
    
    public string Command => CommandList.AddStory;
    public bool SupportSingleLineMode => true;

    public AddStoryCommandCreator(ITeamAccessor teamAccessor)
    {
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }
    
    public async Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        var separator = ' ';
        var teammates = await _teamAccessor.GetTeammates(teamContext.TeamId, DateTimeOffset.UtcNow, token);
        var storyItems = messageContext.Text.Split(separator);
        var links = new List<string>();
        var text = new List<string>();

        foreach (var storyItem in storyItems)
        {
            if (GlobalSettings.LinksPrefix.Any(l => storyItem.StartsWith(l, StringComparison.InvariantCultureIgnoreCase)))
                links.Add(storyItem.ToLower());
            else
                text.Add(storyItem);
        }
            
        return new AddStoryCommand(
            messageContext,
            teamContext.TeamId,
            teamContext.GetStoryType(),
            string.Join(separator, text),
            links,
            teammates);
    }
}