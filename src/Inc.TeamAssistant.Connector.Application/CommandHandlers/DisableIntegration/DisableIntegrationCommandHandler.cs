using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.DisableIntegration;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.DisableIntegration;

internal sealed class DisableIntegrationCommandHandler : IRequestHandler<DisableIntegrationCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;
    private readonly ITeamReader _teamReader;

    public DisableIntegrationCommandHandler(
        ITeamRepository teamRepository,
        ICurrentPersonResolver currentPersonResolver,
        ITeamReader teamReader)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _currentPersonResolver =
            currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
        _teamReader = teamReader ?? throw new ArgumentNullException(nameof(teamReader));
    }

    public async Task Handle(DisableIntegrationCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var team = await _teamRepository.Find(command.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);
        
        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        if (!await _teamReader.HasManagerAccess(team.Id, currentPerson.Id, token))
            throw new ApplicationException(
                $"User {currentPerson.DisplayName} has not rights to remove teammate from team {command.TeamId}");
        
        team.RemoveProperty(PropertyKey.AccessToken, PropertyKey.ProjectKey, PropertyKey.ScrumMaster);

        await _teamRepository.Upsert(team, token);
    }
}