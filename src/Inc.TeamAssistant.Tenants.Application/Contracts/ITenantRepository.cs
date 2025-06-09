using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface ITenantRepository
{
    Task<Tenant?> FindTenant(long ownerId, CancellationToken token);
    
    Task<Room?> FindRoom(Guid id, CancellationToken token);

    Task Upsert(Room room, CancellationToken token);

    Task Remove(Room room, CancellationToken token);
}