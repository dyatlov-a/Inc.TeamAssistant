namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record StoryDto(
    Guid Id,
    string Title,
    IReadOnlyCollection<StoryForEstimateDto> StoryForEstimates,
    bool EstimateEnded,
    string Mean,
    string Median,
    string AcceptedValue,
    int RoundsCount,
    string? Url);