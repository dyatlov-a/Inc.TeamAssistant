namespace Inc.TeamAssistant.Appraiser.Model.CheckIn.Queries.GetLocations;

public sealed record GetLocationsResult(Dictionary<string, IReadOnlyCollection<LocationDto>> Locations)
{
    public IEnumerable<KeyValuePair<string, IReadOnlyCollection<LocationDto>>> OrderedLocations() => Locations.OrderBy(l => l.Key);

    public bool HasHistory(IReadOnlyCollection<LocationDto> locations)
    {
        if (locations is null)
            throw new ArgumentNullException(nameof(locations));

        return locations.Count > 1;
    }
}