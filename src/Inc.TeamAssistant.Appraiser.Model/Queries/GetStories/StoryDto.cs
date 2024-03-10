namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;

public sealed record StoryDto(Guid Id, DateTimeOffset Created, string Title);