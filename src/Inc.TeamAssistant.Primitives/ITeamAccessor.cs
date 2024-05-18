using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives;

public interface ITeamAccessor
{
    Task<(Guid BotId, string TeamName)> GetTeamContext(Guid teamId, CancellationToken token);
    Task<IReadOnlyCollection<Person>> GetTeammates(Guid teamId, CancellationToken token);
    Task<Person?> FindPerson(long personId, CancellationToken token);
    Task<LanguageId> GetClientLanguage(long personId, CancellationToken token);
}