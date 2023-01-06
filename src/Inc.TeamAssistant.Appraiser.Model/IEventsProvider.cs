namespace Inc.TeamAssistant.Appraiser.Model;

public interface IEventsProvider : IAsyncDisposable
{
    Task OnStoryChanged(Guid assessmentSessionId, Func<Task> changed);
}