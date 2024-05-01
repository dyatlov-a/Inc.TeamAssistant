namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record StoryDetailsDto(
    Guid Id,
    string Title,
    IReadOnlyCollection<string> Links,
    IReadOnlyCollection<StoryForEstimateDto> StoryForEstimates,
    string Total);