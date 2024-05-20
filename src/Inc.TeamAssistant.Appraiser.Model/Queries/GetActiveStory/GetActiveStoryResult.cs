using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;

public sealed record GetActiveStoryResult(string TeamName, string CodeForConnect, StoryDto? Story);