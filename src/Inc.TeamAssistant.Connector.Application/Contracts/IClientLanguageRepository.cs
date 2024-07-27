using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IClientLanguageRepository
{
    Task<LanguageId> Get(Guid botId, long personId, CancellationToken token);
    
    Task Upsert(Guid botId, long personId, string languageId, DateTimeOffset now, CancellationToken token);
}