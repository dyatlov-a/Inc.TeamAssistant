using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroHubClient
{
    Task RetroItemChanged(RetroItemDto item);
    
    Task RetroItemRemoved(Guid itemId);

    Task RetroSessionChanged(RetroSessionDto session);
    
    Task VotesChanged(long personId, int votesCount);
    
    Task RetroStateChanged(long personId, bool finished, bool handRaised);
    
    Task PersonsChanged(IReadOnlyCollection<Person> persons);

    Task ItemMoved(Guid itemId);
    
    Task ActionItemChanged(ActionItemDto item);

    Task ActionItemRemoved(Guid itemId);
    
    Task TimerChanged(TimeSpan? duration);
    
    Task RetroPropertiesChanged(RetroPropertiesDto properties, IReadOnlyCollection<RetroColumnDto> columns);
}