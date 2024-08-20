namespace Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

public sealed record LocationDto(
    long PersonId,
    string DisplayName,
    double Longitude,
    double Latitude,
    TimeSpan? UtcOffset,
    string DisplayOffset,
    string CountryName);