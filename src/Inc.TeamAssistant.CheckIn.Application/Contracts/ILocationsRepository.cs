using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Primitives;

namespace Inc.TeamAssistant.CheckIn.Application.Contracts;

public interface ILocationsRepository
{
    Task<Map?> Find(long chatId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LocationOnMap>> GetLocations(MapId mapId, CancellationToken cancellationToken);

    Task Insert(LocationOnMap locationOnMap, CancellationToken cancellationToken);
}