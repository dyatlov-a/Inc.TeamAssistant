using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;

public sealed record GetStoriesResult(IReadOnlyCollection<StoryDto> Items);