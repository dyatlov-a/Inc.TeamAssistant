using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;
using Inc.TeamAssistant.Primitives.Exceptions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.SetIntegrationProperties;

internal sealed class SetIntegrationPropertiesCommandHandler : IRequestHandler<SetIntegrationPropertiesCommand>
{
    private readonly ITeamRepository _teamRepository;

    public SetIntegrationPropertiesCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
    }

    public async Task Handle(SetIntegrationPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);

        team
            .ChangeProperty("accessToken", command.AccessToken)
            .ChangeProperty("projectKey", command.ProjectKey)
            .ChangeProperty("scrumMasterId", command.ScrumMasterId.ToString());

        await _teamRepository.Upsert(team, token);
    }
}