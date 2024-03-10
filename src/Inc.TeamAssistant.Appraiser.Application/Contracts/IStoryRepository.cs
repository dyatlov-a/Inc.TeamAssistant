using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IStoryRepository
{
    Task<IReadOnlyCollection<AssessmentHistoryDto>> GetAssessmentHistory(
        Guid teamId,
        int depth,
        CancellationToken token);
    
    Task<IReadOnlyCollection<StoryDto>> GetStories(Guid teamId, DateOnly assessmentDate, CancellationToken token);
    
    Task<Story?> FindLast(Guid teamId, CancellationToken token);
    
    Task<Story?> Find(Guid storyId, CancellationToken token);
    
    Task Upsert(Story story, CancellationToken token);
}