using Inc.TeamAssistant.Retro.Model.Common;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroEventSender
{
    Task RetroItemChanged(RetroItemDto item);

    Task RetroItemRemoved(RetroItemDto item);
    
    Task RetroSessionChanged(RetroSessionDto session);
}