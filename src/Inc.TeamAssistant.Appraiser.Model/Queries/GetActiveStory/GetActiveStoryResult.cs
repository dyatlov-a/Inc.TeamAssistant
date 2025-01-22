using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;

public sealed record GetActiveStoryResult(string TeamName, string CodeForConnect, StoryDto? Story)
    : IWithEmpty<GetActiveStoryResult>
{
    public static GetActiveStoryResult Empty { get; } = new(string.Empty, string.Empty, Story: null);
}