namespace Inc.TeamAssistant.Primitives.Features.Tenants;

public interface IOnlinePersonStore
{
    string? FindConnectionId(RoomId roomId, long personId);
    
    IReadOnlyCollection<Person> GetPersons(RoomId roomId);
    
    void JoinToRoom(RoomId roomId, string connectionId, Person person);

    IEnumerable<RoomId> LeaveFromRooms(string connectionId);
}