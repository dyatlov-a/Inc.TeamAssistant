namespace Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

public sealed record LocationDto(
    long PersonId,
    string PersonDisplayName,
    string AvatarUrl,
    double Longitude,
    double Latitude,
    string DisplayTimeOffset,
    string CountryName,
    string WorkSchedule,
    IReadOnlyCollection<PersonStats> Stats);