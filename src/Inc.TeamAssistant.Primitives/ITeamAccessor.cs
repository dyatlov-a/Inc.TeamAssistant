using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives;

public interface ITeamAccessor
{
    Task<IReadOnlyCollection<Person>> GetTeammates(Guid teamId, CancellationToken token);
    Task<Person?> FindPerson(long personId, CancellationToken token);
    Task<LanguageId> GetClientLanguage(long personId, CancellationToken token);
}