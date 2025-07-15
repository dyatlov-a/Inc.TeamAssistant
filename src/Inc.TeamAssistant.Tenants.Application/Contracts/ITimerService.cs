namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface ITimerService
{
    void Start(Guid roomId, TimeSpan duration);

    void Stop(Guid roomId);

    TimeSpan? TryGetValue(Guid roomId);
}