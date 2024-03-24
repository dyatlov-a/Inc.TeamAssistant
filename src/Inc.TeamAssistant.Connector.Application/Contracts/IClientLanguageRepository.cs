using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IClientLanguageRepository
{
    Task<LanguageId> Get(long personId, CancellationToken token);
    
    Task Upsert(long personId, string languageId, CancellationToken token);
}