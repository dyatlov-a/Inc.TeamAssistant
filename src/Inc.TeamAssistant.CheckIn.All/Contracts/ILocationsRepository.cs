using Inc.TeamAssistant.CheckIn.All.Model;

namespace Inc.TeamAssistant.CheckIn.All.Contracts;

public interface ILocationsRepository
{
    Task<Map?> Find(long chatId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LocationOnMap>> GetLocations(Guid mapId, CancellationToken cancellationToken);

    Task Insert(LocationOnMap locationOnMap, CancellationToken cancellationToken);
}