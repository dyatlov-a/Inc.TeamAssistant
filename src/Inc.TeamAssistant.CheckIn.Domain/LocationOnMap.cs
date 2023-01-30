using Inc.TeamAssistant.CheckIn.Primitives;

namespace Inc.TeamAssistant.CheckIn.Domain;

public sealed class LocationOnMap
{
    public LocationOnMapId Id { get; private set; } = default!;
    public long UserId { get; private set; }
    public string DisplayName { get; private set; } = default!;
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public string Data { get; private set; } = default!;
    public Map Map { get; private set; } = default!;

    private LocationOnMap()
    {
    }

    public LocationOnMap(long userId, string displayName, double longitude, double latitude, string data, Map map)
        : this()
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(displayName));
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(data));

        Id = new LocationOnMapId(Guid.NewGuid());
        UserId = userId;
        DisplayName = displayName;
        Longitude = longitude;
        Latitude = latitude;
        Created = DateTimeOffset.UtcNow;
        Data = data;
        Map = map ?? throw new ArgumentNullException(nameof(map));
    }
}