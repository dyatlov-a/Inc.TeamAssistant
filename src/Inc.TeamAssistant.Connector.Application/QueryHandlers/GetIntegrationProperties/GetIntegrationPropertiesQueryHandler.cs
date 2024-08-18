using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetIntegrationProperties;

internal sealed class GetIntegrationPropertiesQueryHandler
    : IRequestHandler<GetIntegrationPropertiesQuery, GetIntegrationPropertiesResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;
    private readonly ITeamReader _teamReader;

    public GetIntegrationPropertiesQueryHandler(
        ITeamRepository teamRepository,
        ICurrentPersonResolver currentPersonResolver,
        ITeamReader teamReader)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _currentPersonResolver =
            currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
        _teamReader = teamReader ?? throw new ArgumentNullException(nameof(teamReader));
    }

    public async Task<GetIntegrationPropertiesResult> Handle(
        GetIntegrationPropertiesQuery query,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var team = await _teamRepository.Find(query.TeamId, token);
        if (team is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, query.TeamId);
        
        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        var hasManagerAccess = await _teamReader.HasManagerAccess(team.Id, currentPerson.Id, token);
        var scrumMasterId = team.Properties.TryGetValue("scrumMaster", out var scrumMaster) && long.TryParse(scrumMaster, out var value)
            ? value
            : (long?)null;
        var teammates = team.Teammates
            .Append(team.Owner)
            .DistinctBy(t => t.Id)
            .OrderBy(t => t.DisplayName)
            .ToArray();
        var integrationProperties = scrumMasterId is not null
            ? new IntegrationProperties(
                team.Properties.GetValueOrDefault("accessToken", string.Empty),
                team.Properties.GetValueOrDefault("projectKey", string.Empty),
                scrumMasterId.Value)
            : null;
        
        return new GetIntegrationPropertiesResult(
            integrationProperties,
            hasManagerAccess,
            teammates);
    }
}