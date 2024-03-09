namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record StoryDetailsDto(
    string Title,
    IReadOnlyCollection<string> Links,
    IReadOnlyCollection<StoryForEstimateDto> StoryForEstimates,
    string Total);