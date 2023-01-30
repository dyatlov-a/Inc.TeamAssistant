namespace Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

public sealed record LocationDto(
    string DisplayName,
    double Longitude,
    double Latitude,
    TimeSpan? UtcOffset);