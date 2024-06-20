using Inc.TeamAssistant.CheckIn.Domain;

namespace Inc.TeamAssistant.CheckIn.Application.Contracts;

public interface ILocationsRepository
{
    Task<IReadOnlyCollection<Map>> GetByBot(Guid botId, CancellationToken token);
    
    Task<Map?> Find(long chatId, CancellationToken token);

    Task<IReadOnlyCollection<LocationOnMap>> GetLocations(Guid mapId, CancellationToken token);

    Task Insert(LocationOnMap locationOnMap, CancellationToken token);
}