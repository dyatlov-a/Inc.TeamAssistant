namespace Inc.TeamAssistant.WebUI.Routing;

internal sealed class RouterScope : IDisposable
{
    private readonly Action _action;

    public RouterScope(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action();
    }
}