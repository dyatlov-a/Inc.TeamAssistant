namespace Inc.TeamAssistant.Primitives.Features.Tenants;

public interface IPersonState
{
    IReadOnlyCollection<PersonStateTicket> Get(RoomId roomId);
    
    void Set(RoomId roomId, PersonStateTicket ticket);
    
    void Clear(RoomId roomId);
}