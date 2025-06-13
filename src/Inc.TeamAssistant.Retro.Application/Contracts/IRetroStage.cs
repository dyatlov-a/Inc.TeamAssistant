using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroStage
{
    IReadOnlyCollection<RetroStageTicket> Get(Guid roomId);
    
    void Set(Guid roomId, RetroStageTicket ticket);
    
    void Clear(Guid roomId);
}