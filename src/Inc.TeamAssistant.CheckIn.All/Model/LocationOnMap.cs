namespace Inc.TeamAssistant.CheckIn.All.Model;

public sealed class LocationOnMap
{
    public Guid Id { get; private set; }
    public long UserId { get; private set; }
    public string DisplayName { get; private set; } = default!;
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public string Data { get; private set; } = default!;

    public Guid MapId { get; private set; }
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

        Id = Guid.NewGuid();
        UserId = userId;
        DisplayName = displayName;
        Longitude = longitude;
        Latitude = latitude;
        Created = DateTimeOffset.UtcNow;
        Data = data;
        Map = map ?? throw new ArgumentNullException(nameof(map));
        MapId = map.Id;
    }
}