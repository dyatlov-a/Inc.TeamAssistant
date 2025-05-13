using Inc.TeamAssistant.Retro.Model.Common;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroHubClient
{
    Task RetroItemChanged(RetroItemDto item);
    
    Task RetroItemRemoved(RetroItemDto item);

    Task RetroSessionChanged(RetroSessionDto session);
}