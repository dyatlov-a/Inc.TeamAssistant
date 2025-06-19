namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface ITimerService
{
    void Start(Guid roomId, TimeSpan duration);

    void Stop(Guid roomId);

    TimeSpan? TryGetValue(Guid roomId);
}