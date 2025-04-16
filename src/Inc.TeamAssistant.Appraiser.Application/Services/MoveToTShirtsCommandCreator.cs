using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Features.Teams;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class MoveToTShirtsCommandCreator : ICommandCreator
{
    private readonly IChangeTeamPropertyCommandFactory _commandFactory;

    public MoveToTShirtsCommandCreator(IChangeTeamPropertyCommandFactory commandFactory)
    {
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
    }

    public string Command => CommandList.MoveToTShirts;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        var command = _commandFactory.Create(
            messageContext,
            AppraiserProperties.StoryTypeKey,
            StoryType.TShirt.ToString());

        return Task.FromResult(command);
    }
}