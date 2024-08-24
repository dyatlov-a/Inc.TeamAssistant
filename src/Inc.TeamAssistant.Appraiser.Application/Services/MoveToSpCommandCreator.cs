using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Handlers;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class MoveToSpCommandCreator : ICommandCreator
{
    private readonly IChangeTeamPropertyCommandFactory _commandFactory;

    public MoveToSpCommandCreator(IChangeTeamPropertyCommandFactory commandFactory)
    {
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
    }

    public string Command => CommandList.MoveToSp;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        var command = _commandFactory.Create(
            messageContext,
            TeamProperties.StoryTypeKey,
            StoryType.Scrum.ToString());

        return Task.FromResult(command);
    }
}