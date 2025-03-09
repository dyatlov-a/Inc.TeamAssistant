using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.SetIntegrationProperties;

internal sealed class SetIntegrationPropertiesCommandHandler : IRequestHandler<SetIntegrationPropertiesCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;
    private readonly ITeamAccessor _teamAccessor;

    public SetIntegrationPropertiesCommandHandler(
        ITeamRepository teamRepository,
        ICurrentPersonResolver currentPersonResolver,
        ITeamAccessor teamAccessor)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _currentPersonResolver =
            currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task Handle(SetIntegrationPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);
        
        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        var hasManagerAccess = await _teamAccessor.HasManagerAccess(team.Id, currentPerson.Id, token);
        if (!hasManagerAccess)
            throw new ApplicationException(
                $"User {currentPerson.DisplayName} has not rights to remove teammate from team {command.TeamId}");
        
        var accessToken = team.Properties.GetPropertyValueOrDefault(
            ConnectorProperties.AccessToken,
            Guid.NewGuid().ToLinkSegment());
        
        team
            .ChangeProperty(ConnectorProperties.AccessToken, accessToken)
            .ChangeProperty(ConnectorProperties.ProjectKey, command.ProjectKey)
            .ChangeProperty(ConnectorProperties.ScrumMaster, command.ScrumMasterId.ToString());

        await _teamRepository.Upsert(team, token);
    }
}