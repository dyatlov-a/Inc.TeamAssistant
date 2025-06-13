namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroPropertiesProvider
{
    Task<RetroProperties> Get(Guid roomId, CancellationToken token);
    
    Task Set(Guid roomId, RetroProperties properties, CancellationToken token);
}