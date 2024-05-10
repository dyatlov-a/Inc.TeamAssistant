namespace Inc.TeamAssistant.Primitives.Bots;

public interface IBotListener
{
    Task Start(Guid botId);
    
    Task Restart(Guid botId);
    
    Task Stop(Guid botId);
}