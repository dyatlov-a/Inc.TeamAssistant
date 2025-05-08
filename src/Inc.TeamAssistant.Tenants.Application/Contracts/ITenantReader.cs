using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface ITenantReader
{
    Task<IReadOnlyCollection<Team>> GetAvailableTeams(Guid? teamId, long personId, CancellationToken token);
}