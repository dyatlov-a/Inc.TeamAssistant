using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;

internal sealed class AddStoryCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.AddStory;

    public IDialogCommand? TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        if (!command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return null;

        var separator = ' ';
        var storyItems = messageContext.Text.Split(separator);
        var links = new List<string>();
        var text = new List<string>();

        foreach (var storyItem in storyItems)
        {
            if (GlobalResources.Settings.LinksPrefix.Any(l => storyItem.StartsWith(
                    l,
                    StringComparison.InvariantCultureIgnoreCase)))
                links.Add(storyItem.ToLowerInvariant());
            else
                text.Add(storyItem);
        }

        return new AddStoryCommand(
            messageContext,
            teamContext.TeamId,
            teamContext.Properties.GetStoryType(),
            string.Join(separator, text),
            links);
    }
}