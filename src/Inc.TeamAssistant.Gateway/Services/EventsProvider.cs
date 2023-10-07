using Inc.TeamAssistant.Appraiser.Model;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class EventsProvider : IEventsProvider
{
    public Task OnStoryChanged(Guid assessmentSessionId, Func<Task> changed) => Task.CompletedTask;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}