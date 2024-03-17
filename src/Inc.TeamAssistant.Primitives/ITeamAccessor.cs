namespace Inc.TeamAssistant.Primitives;

public interface ITeamAccessor
{
    Task<IReadOnlyCollection<(long PersonId, string PersonDisplayName)>> GetTeammates(
        Guid teamId,
        CancellationToken token);

    Task<Person?> FindPerson(long personId, CancellationToken token);
}