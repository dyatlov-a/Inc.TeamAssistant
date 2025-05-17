using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.SetIntegrationProperties;

internal sealed class SetIntegrationPropertiesCommandHandler : IRequestHandler<SetIntegrationPropertiesCommand>
{
    private readonly ITeamRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly ITeamAccessor _teamAccessor;

    public SetIntegrationPropertiesCommandHandler(
        ITeamRepository repository,
        IPersonResolver personResolver,
        ITeamAccessor teamAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task Handle(SetIntegrationPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var team = await command.TeamId.Required(_repository.Find, token);
        var currentPerson = _personResolver.GetCurrentPerson();
        
        await _teamAccessor.EnsureManagerAccess(new(command.TeamId, currentPerson.Id), token);
        
        var accessToken = team.Properties.GetPropertyValueOrDefault(
            ConnectorProperties.AccessToken,
            Guid.NewGuid().ToString("N"));
        
        team
            .ChangeProperty(ConnectorProperties.AccessToken, accessToken)
            .ChangeProperty(ConnectorProperties.ProjectKey, command.ProjectKey)
            .ChangeProperty(ConnectorProperties.ScrumMaster, command.ScrumMasterId.ToString());

        await _repository.Upsert(team, token);
    }
}