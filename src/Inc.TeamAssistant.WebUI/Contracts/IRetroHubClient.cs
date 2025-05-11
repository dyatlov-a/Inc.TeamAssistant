using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroHubClient
{
    Task RetroItemChanged(RetroItemDto item);
    
    Task RetroItemRemoved(RetroItemDto item);
}