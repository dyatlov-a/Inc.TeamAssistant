namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

internal sealed class Scope : IAsyncDisposable
{
    private readonly Func<Task> _action;

    public Scope(Func<Task> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public async ValueTask DisposeAsync() => await _action();
}