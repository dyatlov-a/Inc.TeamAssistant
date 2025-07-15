using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface IPersonRoomState
{
    IReadOnlyCollection<PersonRoomTicket> Get(Guid roomId);
    
    void Set(Guid roomId, PersonRoomTicket ticket);
    
    void Clear(Guid roomId);
}