using System.Timers;

namespace Inc.TeamAssistant.WebUI.Features.Notifications;

internal sealed class TimerScope : IDisposable
{
    private readonly Action _action;
    private readonly System.Timers.Timer _timer;
    
    public TimerScope(Action action, TimeSpan checkInterval)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _timer = new System.Timers.Timer(checkInterval);
        _timer.Elapsed += Tick;
        _timer.Enabled = true;
    }
    
    private void Tick(object? sender, ElapsedEventArgs e) => _action();

    public void Dispose()
    {
        _timer.Elapsed -= Tick;
        _timer.Dispose();
    }
}