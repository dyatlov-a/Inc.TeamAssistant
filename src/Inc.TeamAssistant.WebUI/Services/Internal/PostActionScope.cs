namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal sealed class PostActionScope : IAsyncDisposable
{
    private readonly Func<Task> _action;

    public PostActionScope(Func<Task> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public async ValueTask DisposeAsync() => await _action();
}