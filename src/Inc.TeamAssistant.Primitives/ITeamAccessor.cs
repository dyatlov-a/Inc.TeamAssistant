using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives;

public interface ITeamAccessor
{
    Task<(Guid BotId, string TeamName)> GetTeamContext(Guid teamId, CancellationToken token);
    Task<IReadOnlyCollection<Person>> GetTeammates(Guid teamId, DateTimeOffset now, CancellationToken token);
    Task<Person?> FindPerson(long personId, CancellationToken token);
    Task<Person> GetPerson(long personId, CancellationToken token);
    Task<LanguageId> GetClientLanguage(Guid botId, long personId, CancellationToken token);
    Task<bool> HasManagerAccess(Guid teamId, long personId, CancellationToken token);
}