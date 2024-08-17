using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.DisableIntegration;
using Inc.TeamAssistant.Primitives.Exceptions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.DisableIntegration;

internal sealed class DisableIntegrationCommandHandler : IRequestHandler<DisableIntegrationCommand>
{
    private readonly ITeamRepository _teamRepository;

    public DisableIntegrationCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
    }

    public async Task Handle(DisableIntegrationCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);
        
        team
            .RemoveProperty("accessToken")
            .RemoveProperty("projectKey")
            .RemoveProperty("scrumMasterId");

        await _teamRepository.Upsert(team, token);
    }
}