using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroStage
{
    IReadOnlyCollection<RetroStageTicket> Get(Guid teamId);
    
    void Set(Guid teamId, RetroStageTicket ticket);
    
    void Clear(Guid teamId);
}