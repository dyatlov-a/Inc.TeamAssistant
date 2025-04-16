namespace Inc.TeamAssistant.Primitives.Bots;

public interface IBotListeners
{
    Task Start(Guid botId, CancellationToken token);
    
    Task Restart(Guid botId, CancellationToken token);
    
    Task Stop(Guid botId, CancellationToken token);

    Task Shutdown(CancellationToken token);
}