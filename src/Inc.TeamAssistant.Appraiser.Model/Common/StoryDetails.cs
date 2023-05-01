namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record StoryDetails(int ExternalId, string Title, IReadOnlyCollection<string> Links);