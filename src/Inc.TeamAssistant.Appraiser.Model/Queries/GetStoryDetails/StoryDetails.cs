namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

public sealed record StoryDetails(string Title, IReadOnlyCollection<string> Links);