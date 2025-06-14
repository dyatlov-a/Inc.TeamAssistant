using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroEventSender
{
    Task RetroItemChanged(RetroItemDto item, bool excludedOwner = false);

    Task RetroItemRemoved(Guid roomId, Guid itemId);
    
    Task RetroSessionChanged(RetroSessionDto session);
    
    Task VotesChanged(Guid roomId, long personId, int votesCount);
    
    Task RetroStateChanged(Guid roomId, long personId, bool finished, bool handRaised);

    Task PersonsChanged(Guid roomId, IReadOnlyCollection<Person> persons);

    Task ActionItemChanged(Guid roomId, ActionItemDto item);
    
    Task ActionItemRemoved(Guid roomId, Guid itemId, string connectionId);
    
    Task RoomPropertiesChanged(Guid roomId);

    Task TimerChanged(Guid roomId, TimeSpan? duration);
}