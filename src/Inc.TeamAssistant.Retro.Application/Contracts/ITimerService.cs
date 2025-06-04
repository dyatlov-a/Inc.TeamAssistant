namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface ITimerService
{
    void Start(Guid teamId, TimeSpan duration);

    void Stop(Guid teamId);

    TimeSpan? TryGetValue(Guid teamId);
}