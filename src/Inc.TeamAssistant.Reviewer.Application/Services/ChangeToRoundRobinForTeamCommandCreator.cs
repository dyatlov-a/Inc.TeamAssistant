using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ChangeToRoundRobinForTeamCommandCreator : ICommandCreator
{
    private readonly IChangeTeamPropertyCommandFactory _commandFactory;

    public ChangeToRoundRobinForTeamCommandCreator(IChangeTeamPropertyCommandFactory commandFactory)
    {
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
    }

    public string Command => CommandList.ChangeToRoundRobinForTeam;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        var command = _commandFactory.Create(
            messageContext,
            ReviewerProperties.NextReviewerTypeKey,
            NextReviewerType.RoundRobinForTeam.ToString());

        return Task.FromResult(command);
    }
}