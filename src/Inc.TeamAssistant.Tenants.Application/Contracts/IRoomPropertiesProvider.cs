namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface IRoomPropertiesProvider
{
    Task<IReadOnlyDictionary<string, string>> Get(Guid roomId, CancellationToken token);

    Task Set(Guid roomId, IReadOnlyDictionary<string, string> properties, CancellationToken token);
}