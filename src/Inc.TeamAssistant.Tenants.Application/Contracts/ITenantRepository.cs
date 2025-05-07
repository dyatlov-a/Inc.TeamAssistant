using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface ITenantRepository
{
    Task<Team?> Find(Guid id, CancellationToken token);

    Task Upsert(Team team, CancellationToken token);
}