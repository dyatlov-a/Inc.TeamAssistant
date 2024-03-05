namespace Inc.TeamAssistant.Appraiser.Model;

public interface IEventsProvider : IAsyncDisposable
{
    Task OnStoryChanged(Guid teamId, Func<Task> changed);
}