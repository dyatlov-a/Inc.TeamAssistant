namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record StoryForEstimateDto(
    long ParticipantId,
    string ParticipantName,
    string AvatarUrl,
    string DisplayValue,
    int? ValueGroup);