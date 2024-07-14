using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IStoryReader
{
    Task<IReadOnlyCollection<AssessmentHistoryDto>> GetAssessmentHistory(
        Guid teamId,
        DateTimeOffset before,
        DateTimeOffset? from,
        CancellationToken token);
    
    Task<IReadOnlyCollection<Story>> GetStories(Guid teamId, DateOnly assessmentDate, CancellationToken token);
    
    Task<Story?> FindLast(Guid teamId, CancellationToken token);
}