using System.Collections.Concurrent;
using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class TimerInMemoryService : ITimerService
{
    private readonly ConcurrentDictionary<Guid, DateTimeOffset> _timers = new();
    
    public void Start(Guid teamId, TimeSpan duration)
    {
        var end = DateTimeOffset.UtcNow.Add(duration);
        
        _timers.AddOrUpdate(teamId, k => end, (k, v) => end);
    }

    public void Stop(Guid teamId) => _timers.TryRemove(teamId, out _);

    public TimeSpan? TryGetValue(Guid teamId)
    {
        if (_timers.TryGetValue(teamId, out var value))
        {
            var duration = value - DateTimeOffset.UtcNow;
            if (duration > TimeSpan.Zero)
                return duration;
            
            _timers.TryRemove(teamId, out _);
        }

        return null;
    }
}