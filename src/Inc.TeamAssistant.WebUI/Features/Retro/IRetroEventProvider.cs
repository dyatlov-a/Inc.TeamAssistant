using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public interface IRetroEventProvider
{
    IDisposable OnRetroItemChanged(Func<RetroItemDto, Task> changed);
    
    IDisposable OnRetroItemRemoved(Func<Guid, Task> removed);
    
    IDisposable OnRetroSessionChanged(Func<RetroSessionDto, Task> changed);
    
    IDisposable OnVotesChanged(Func<long, int, Task> changed);
    
    IDisposable OnPersonsChanged(Func<IReadOnlyCollection<Person>, Task> changed);
    
    IDisposable OnItemMoved(Func<Guid, Task> moved);
    
    IDisposable OnActionItemChanged(Func<ActionItemDto, Task> changed);

    IDisposable OnActionItemRemoved(Func<Guid, Task> removed);
    
    IDisposable OnTimerChanged(Func<TimeSpan?, Task> changed);
}