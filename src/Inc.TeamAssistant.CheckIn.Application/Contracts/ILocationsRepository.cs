using Inc.TeamAssistant.CheckIn.Domain;

namespace Inc.TeamAssistant.CheckIn.Application.Contracts;

public interface ILocationsRepository
{
    Task<Map?> Find(long chatId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LocationOnMap>> GetLocations(Guid mapId, CancellationToken cancellationToken);

    Task Insert(LocationOnMap locationOnMap, CancellationToken cancellationToken);
}