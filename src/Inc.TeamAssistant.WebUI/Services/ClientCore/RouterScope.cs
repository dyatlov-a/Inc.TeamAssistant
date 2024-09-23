namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

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