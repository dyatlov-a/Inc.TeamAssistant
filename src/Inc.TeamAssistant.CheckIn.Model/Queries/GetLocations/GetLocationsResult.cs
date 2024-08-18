namespace Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

public sealed record GetLocationsResult(IReadOnlyDictionary<string, IReadOnlyCollection<LocationDto>> Locations);