using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

namespace Inc.TeamAssistant.Gateway.Services.CheckIn;

internal sealed class DummyLocationBuilder : ILocationBuilder
{
    public Task AddMarker(
        LocationDto location,
        bool hasHistory,
        int index,
        bool isActual,
        CancellationToken cancellationToken) => Task.CompletedTask;

    public Task AddLayer(string title, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task AddRoute(string title, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task Build(CancellationToken cancellationToken = default) => Task.CompletedTask;
}