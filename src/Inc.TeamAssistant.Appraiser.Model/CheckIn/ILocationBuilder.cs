using Inc.TeamAssistant.Appraiser.Model.CheckIn.Queries.GetLocations;

namespace Inc.TeamAssistant.Appraiser.Model.CheckIn;

public interface ILocationBuilder
{
    Task AddMarker(
        LocationDto location,
        bool hasHistory = false,
        int index = 0,
        bool isActual = true,
        CancellationToken cancellationToken = default);

    Task AddLayer(string title, CancellationToken cancellationToken = default);

    Task AddRoute(string title, CancellationToken cancellationToken = default);

    Task Build(CancellationToken cancellationToken = default);
}