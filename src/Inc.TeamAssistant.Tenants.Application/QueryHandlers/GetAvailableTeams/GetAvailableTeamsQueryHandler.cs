using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Queries.Common;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableTeams;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetAvailableTeams;

internal sealed class GetAvailableTeamsQueryHandler : IRequestHandler<GetAvailableTeamsQuery, GetAvailableTeamsResult>
{
    private readonly ITenantReader _tenantReader;
    private readonly IPersonResolver _personResolver;

    public GetAvailableTeamsQueryHandler(ITenantReader tenantReader, IPersonResolver personResolver)
    {
        _tenantReader = tenantReader ?? throw new ArgumentNullException(nameof(tenantReader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task<GetAvailableTeamsResult> Handle(GetAvailableTeamsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var person = _personResolver.GetCurrentPerson();
        var teams = await _tenantReader.GetAvailableTeams(query.TeamId, person.Id, token);
        
        var results = teams
            .Select(t => new TenantTeamDto(t.Id, t.Name))
            .OrderBy(t => t.Name)
            .ToArray();
        
        return new GetAvailableTeamsResult(results);
    }
}