using Inc.TeamAssistant.Connector.Domain;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IDashboardSettingsRepository
{
    Task<DashboardSettings?> Find(long personId, Guid botId, CancellationToken token);
    
    Task Upsert(DashboardSettings settings, CancellationToken token);
}