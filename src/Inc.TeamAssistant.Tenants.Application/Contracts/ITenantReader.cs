using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface ITenantReader
{
    Task<IReadOnlyCollection<Room>> GetAvailableRooms(Guid? teamId, long personId, CancellationToken token);
}