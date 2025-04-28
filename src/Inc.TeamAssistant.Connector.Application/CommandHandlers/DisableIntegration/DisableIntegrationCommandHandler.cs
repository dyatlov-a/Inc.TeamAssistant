using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.DisableIntegration;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.DisableIntegration;

internal sealed class DisableIntegrationCommandHandler : IRequestHandler<DisableIntegrationCommand>
{
    private readonly ITeamRepository _repository;
    private readonly ICurrentPersonResolver _personResolver;
    private readonly ITeamAccessor _teamAccessor;

    public DisableIntegrationCommandHandler(
        ITeamRepository repository,
        ICurrentPersonResolver personResolver,
        ITeamAccessor teamAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task Handle(DisableIntegrationCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var team = await command.TeamId.Required(_repository.Find, token);
        var currentPerson = _personResolver.GetCurrentPerson();

        await _teamAccessor.EnsureManagerAccess(new(team.Id, currentPerson.Id), token);

        await _repository.Upsert(
            team.RemoveProperty(
                ConnectorProperties.AccessToken,
                ConnectorProperties.ProjectKey,
                ConnectorProperties.ScrumMaster),
            token);
    }
}