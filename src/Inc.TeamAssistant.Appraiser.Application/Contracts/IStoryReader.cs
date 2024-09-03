using Inc.TeamAssistant.Appraiser.Domain;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IStoryReader
{
    Task<IReadOnlyCollection<Story>> GetStories(
        Guid teamId,
        DateTimeOffset before,
        DateTimeOffset? from,
        CancellationToken token);
    
    Task<IReadOnlyCollection<Story>> GetStories(Guid teamId, DateOnly assessmentDate, CancellationToken token);
    
    Task<Story?> FindLast(Guid teamId, CancellationToken token);
}