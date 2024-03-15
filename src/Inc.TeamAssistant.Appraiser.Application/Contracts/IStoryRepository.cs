using Inc.TeamAssistant.Appraiser.Domain;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IStoryRepository
{
    Task<Story?> Find(Guid storyId, CancellationToken token);
    
    Task Upsert(Story story, CancellationToken token);
}