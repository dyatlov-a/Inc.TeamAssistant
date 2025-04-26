using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ChangeToRoundRobinCommandCreator : ICommandCreator
{
    private readonly IChangeTeamPropertyCommandFactory _commandFactory;
    private readonly string _command = CommandList.ChangeToRoundRobin;

    public ChangeToRoundRobinCommandCreator(IChangeTeamPropertyCommandFactory commandFactory)
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
            ReviewerProperties.NextReviewerTypeKey,
            nameof(NextReviewerType.RoundRobin));

        return Task.FromResult<IDialogCommand?>(cmd);
    }
}