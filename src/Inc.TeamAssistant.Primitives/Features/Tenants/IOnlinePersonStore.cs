namespace Inc.TeamAssistant.Primitives.Features.Tenants;

public interface IOnlinePersonStore
{
    string? FindConnectionId(RoomId roomId, long personId);
    
    IReadOnlyCollection<Person> GetPersons(RoomId roomId);
    
    IReadOnlyCollection<Person> JoinToRoom(RoomId roomId, string connectionId, Person person);

    IReadOnlyCollection<Person> LeaveFromRoom(RoomId roomId, string connectionId);

    IEnumerable<RoomId> LeaveFromRooms(string connectionId);
}