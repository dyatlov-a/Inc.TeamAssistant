using Inc.TeamAssistant.Appraiser.Model.CheckIn;
using Inc.TeamAssistant.Appraiser.Model.CheckIn.Queries.GetLocations;
using Microsoft.JSInterop;

namespace Inc.TeamAssistant.WebUI.Services.CheckIn;

internal sealed class LocationBuilder : ILocationBuilder
{
    private readonly IJSRuntime _jsRuntime;

    public LocationBuilder(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    public async Task AddMarker(
        LocationDto location,
        bool hasHistory,
        int index,
        bool isActual,
        CancellationToken cancellationToken)
    {
        if (location is null)
            throw new ArgumentNullException(nameof(location));
        
        var timeOffset = location.UtcOffset.HasValue
            ? $"{(location.UtcOffset.Value < TimeSpan.Zero ? "-" : "+")}{location.UtcOffset.Value:hh\\:mm}"
            : "?";

        await _jsRuntime.InvokeVoidAsync(
            "locations.builder.addMarker",
            cancellationToken,
            location.DisplayName,
            location.Longitude,
            location.Latitude,
            timeOffset,
            index,
            isActual,
            hasHistory);
    }

    public async Task AddLayer(string title, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

        await _jsRuntime.InvokeVoidAsync("locations.builder.addLayer", cancellationToken, title);
    }
    public async Task AddRoute(string title, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

        await _jsRuntime.InvokeVoidAsync("locations.builder.addRoute", cancellationToken, title);
    }

    public async Task Build(CancellationToken cancellationToken)
    {
        await _jsRuntime.InvokeVoidAsync("locations.builder.build", cancellationToken);
    }
}