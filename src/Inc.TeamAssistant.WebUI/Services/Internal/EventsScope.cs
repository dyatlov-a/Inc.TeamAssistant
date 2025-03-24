namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal sealed class EventsScope : IAsyncDisposable
{
    private readonly Func<Task> _action;

    public EventsScope(Func<Task> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public async ValueTask DisposeAsync() => await _action();
}