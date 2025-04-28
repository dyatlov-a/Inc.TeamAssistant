using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Features.Teams;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class MoveToFibonacciCommandCreator : ICommandCreator
{
    private readonly IChangeTeamPropertyCommandFactory _commandFactory;
    private readonly string _command = CommandList.MoveToFibonacci;

    public MoveToFibonacciCommandCreator(IChangeTeamPropertyCommandFactory commandFactory)
    {
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
    }
    
    public IDialogCommand? TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        if (singleLineMode || !command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return null;
        
        return _commandFactory.Create(messageContext, AppraiserProperties.StoryTypeKey, nameof(StoryType.Fibonacci));
    }
}