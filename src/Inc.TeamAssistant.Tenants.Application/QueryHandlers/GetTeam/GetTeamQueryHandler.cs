using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Queries.GetTeam;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetTeam;

internal sealed class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, GetTeamResult>
{
    private readonly ITenantRepository _repository;

    public GetTeamQueryHandler(ITenantRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task<GetTeamResult> Handle(GetTeamQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var team = await query.Id.Required(_repository.Find, token);

        return new GetTeamResult(team.Id, team.Name);
    }
}