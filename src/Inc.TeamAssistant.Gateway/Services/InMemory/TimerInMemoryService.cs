using System.Collections.Concurrent;
using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class TimerInMemoryService : ITimerService
{
    private readonly ConcurrentDictionary<Guid, DateTimeOffset> _timers = new();
    
    public void Start(Guid roomId, TimeSpan duration)
    {
        var end = DateTimeOffset.UtcNow.Add(duration);
        
        _timers.AddOrUpdate(roomId, k => end, (k, v) => end);
    }

    public void Stop(Guid roomId) => _timers.TryRemove(roomId, out _);

    public TimeSpan? TryGetValue(Guid roomId)
    {
        if (_timers.TryGetValue(roomId, out var value))
        {
            var duration = value - DateTimeOffset.UtcNow;
            if (duration > TimeSpan.Zero)
                return duration;
            
            _timers.TryRemove(roomId, out _);
        }

        return null;
    }
}