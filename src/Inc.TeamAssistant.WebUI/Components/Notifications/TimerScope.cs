using System.Timers;

namespace Inc.TeamAssistant.WebUI.Components.Notifications;

internal sealed class TimerScope : IDisposable
{
    private readonly Action _action;
    private readonly System.Timers.Timer _timer;
    
    public TimerScope(Action action, TimeSpan interval)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _timer = CreateTimer(interval);
        _timer.Start();
    }
    
    private void HandleTick(object? sender, ElapsedEventArgs e) => _action();

    private System.Timers.Timer CreateTimer(TimeSpan interval)
    {
        var timer = new System.Timers.Timer(interval);
        
        timer.Elapsed += HandleTick;

        return timer;
    }
    
    public void Dispose()
    {
        _timer.Stop();
        _timer.Elapsed -= HandleTick;
        _timer.Dispose();
    }
}