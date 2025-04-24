using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives;

public interface ITeamAccessor
{
    Task<CurrentTeamContext> GetTeamContext(Guid teamId, CancellationToken token);
    Task<IReadOnlyCollection<Person>> GetTeammates(Guid teamId, DateTimeOffset now, CancellationToken token);
    Task<IReadOnlyCollection<Person>> GetFinalizes(Guid teamId, DateTimeOffset now, CancellationToken token);
    Task<Person?> FindPerson(long personId, CancellationToken token);
    Task<Person> EnsurePerson(long personId, CancellationToken token);
    Task<LanguageId> GetClientLanguage(Guid botId, long personId, CancellationToken token);
    Task<bool> HasManagerAccess(TeammateKey key, CancellationToken token);
    Task EnsureManagerAccess(TeammateKey key, CancellationToken token);
}