using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Features.Teams;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class MoveToTShirtsCommandCreator : ICommandCreator
{
    private readonly IChangeTeamPropertyCommandFactory _commandFactory;
    private readonly string _command = CommandList.MoveToTShirts;

    public MoveToTShirtsCommandCreator(IChangeTeamPropertyCommandFactory commandFactory)
    {
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
    }
    
    public Task<IDialogCommand?> TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        if (singleLineMode || !command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult<IDialogCommand?>(null);
        
        var cmd = _commandFactory.Create(
            messageContext,
            AppraiserProperties.StoryTypeKey,
            nameof(StoryType.TShirt));

        return Task.FromResult<IDialogCommand?>(cmd);
    }
}