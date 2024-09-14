namespace Inc.TeamAssistant.CheckIn.Domain;

public sealed class LocationOnMap
{
    public Guid Id { get; private set; }
    public long UserId { get; private set; }
    public string DisplayName { get; private set; } = default!;
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public Map Map { get; private set; } = default!;

    private LocationOnMap()
    {
    }

    public LocationOnMap(
        Guid id,
        long userId,
        string displayName,
        double longitude,
        double latitude,
        Map map)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        Id = id;
        UserId = userId;
        DisplayName = displayName;
        Longitude = longitude;
        Latitude = latitude;
        Created = DateTimeOffset.UtcNow;
        Map = map ?? throw new ArgumentNullException(nameof(map));
    }
    
    internal LocationOnMap SetMap(Map map)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));

        return this;
    }
}