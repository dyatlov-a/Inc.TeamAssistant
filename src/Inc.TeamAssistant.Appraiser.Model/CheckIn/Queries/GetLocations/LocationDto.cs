namespace Inc.TeamAssistant.Appraiser.Model.CheckIn.Queries.GetLocations;

public sealed record LocationDto(
    string DisplayName,
    double Longitude,
    double Latitude,
    TimeSpan UtcOffset);