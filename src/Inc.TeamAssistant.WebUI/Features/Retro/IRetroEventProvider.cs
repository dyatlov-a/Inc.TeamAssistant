using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public interface IRetroEventProvider
{
    IDisposable OnRetroItemChanged(Func<RetroItemDto, Task> changed);
    
    IDisposable OnRetroItemRemoved(Func<RetroItemDto, Task> removed);
}