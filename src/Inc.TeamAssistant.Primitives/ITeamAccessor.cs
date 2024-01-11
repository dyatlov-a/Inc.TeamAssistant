namespace Inc.TeamAssistant.Primitives;

public interface ITeamAccessor
{
    Task<IReadOnlyCollection<(long PersonId, string PersonDisplayName)>> GetTeammates(
        Guid teamId,
        CancellationToken token);

    Task<(long Id, string Name, string? Username, LanguageId LanguageId)?> FindPerson(
        long userId,
        CancellationToken token);
    
    Task<(long Id, string Name, string? Username, LanguageId LanguageId)?> FindPerson(
        string username,
        CancellationToken token);
}