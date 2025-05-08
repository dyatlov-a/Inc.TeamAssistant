using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface ITenantRepository
{
    Task<Tenant?> FindTenant(long ownerId, CancellationToken token);
    
    Task<Team?> FindTeam(Guid id, CancellationToken token);

    Task Upsert(Team team, CancellationToken token);

    Task Remove(Team team, CancellationToken token);
}