namespace Inc.TeamAssistant.Primitives.Bots;

public interface IBotListeners
{
    Task Start(Guid botId);
    
    Task Restart(Guid botId);
    
    Task Stop(Guid botId);

    Task Shutdown(CancellationToken token);
}