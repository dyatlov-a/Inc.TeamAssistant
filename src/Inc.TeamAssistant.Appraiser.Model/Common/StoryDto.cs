namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record StoryDto(
    Guid Id,
    string Title,
    IReadOnlyCollection<string> Links,
    IReadOnlyCollection<StoryForEstimateDto> StoryForEstimates,
    bool EstimateEnded,
    string Total,
    bool HasMedian,
    string? Median);