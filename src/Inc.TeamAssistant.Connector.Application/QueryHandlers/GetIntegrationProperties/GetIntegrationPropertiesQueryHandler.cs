using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetIntegrationProperties;

internal sealed class GetIntegrationPropertiesQueryHandler
    : IRequestHandler<GetIntegrationPropertiesQuery, GetIntegrationPropertiesResult>
{
    private readonly ITeamRepository _repository;
    private readonly ICurrentPersonResolver _personResolver;
    private readonly ITeamAccessor _teamAccessor;

    public GetIntegrationPropertiesQueryHandler(
        ITeamRepository repository,
        ICurrentPersonResolver personResolver,
        ITeamAccessor teamAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<GetIntegrationPropertiesResult> Handle(
        GetIntegrationPropertiesQuery query,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var team = await query.TeamId.Required(_repository.Find, token);
        var currentPerson = _personResolver.GetCurrentPerson();
        var hasManagerAccess = await _teamAccessor.HasManagerAccess(new(team.Id, currentPerson.Id), token);
        var scrumMaster = team.Properties.GetPropertyValueOrDefault(ConnectorProperties.ScrumMaster);
        var scrumMasterId = long.TryParse(scrumMaster, out var value)
            ? value
            : (long?)null;
        var teammates = team.Teammates
            .Append(team.Owner)
            .DistinctBy(t => t.Id)
            .OrderBy(t => t.DisplayName)
            .ToArray();
        var integrationProperties = scrumMasterId is not null
            ? new IntegrationProperties(
                team.Properties.GetPropertyValueOrDefault(ConnectorProperties.AccessToken),
                team.Properties.GetPropertyValueOrDefault(ConnectorProperties.ProjectKey),
                scrumMasterId.Value)
            : null;
        
        return new GetIntegrationPropertiesResult(
            integrationProperties,
            hasManagerAccess,
            teammates);
    }
}