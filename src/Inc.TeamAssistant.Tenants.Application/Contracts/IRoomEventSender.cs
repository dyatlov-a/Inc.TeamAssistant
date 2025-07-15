using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Model.Common;

namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface IRoomEventSender
{
    Task PropertiesChanged(Guid roomId, RoomPropertiesDto properties);
    
    Task TimerChanged(Guid roomId, TimeSpan? duration);
    
    Task PersonsChanged(Guid roomId, IReadOnlyCollection<Person> persons);
    
    Task RetroStateChanged(Guid roomId, long personId, bool finished, bool handRaised);
}