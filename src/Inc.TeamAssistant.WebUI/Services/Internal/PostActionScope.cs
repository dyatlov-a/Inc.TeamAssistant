namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal sealed class PostActionScope : IDisposable
{
    private readonly Action _action;

    public PostActionScope(Action action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public void Dispose() => _action();
}