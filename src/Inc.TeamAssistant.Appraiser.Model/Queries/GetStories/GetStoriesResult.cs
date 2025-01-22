using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;

public sealed record GetStoriesResult(IReadOnlyCollection<StoryDto> Items)
    : IWithEmpty<GetStoriesResult>
{
    public static GetStoriesResult Empty { get; } = new(Array.Empty<StoryDto>());
}