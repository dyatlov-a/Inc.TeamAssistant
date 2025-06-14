namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface IRoomPropertiesProvider
{
    Task<T> Get<T>(Guid roomId, CancellationToken token)
        where T : class, new();

    Task Set<T>(Guid roomId, T properties, CancellationToken token)
        where T : class, new();
}