namespace Inc.TeamAssistant.Primitives.Features.Tenants;

public interface IOnlinePersonStore
{
    IReadOnlyCollection<string> GetConnections(RoomId roomId, long personId);
    
    IReadOnlyCollection<PersonStateTicket> GetTickets(RoomId roomId);
    
    void JoinToRoom(RoomId roomId, string connectionId, Person person);

    IEnumerable<RoomId> LeaveFromRooms(string connectionId);
    
    void SetTicket(RoomId roomId, Person person, int totalVote, bool finished, bool handRaised);
    
    void SetTicket(RoomId roomId, Person person, bool finished);
    
    void ClearTickets(RoomId roomId);
}