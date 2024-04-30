using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.Converters;

internal static class StoryConverter
{
    public static StoryDetailsDto Convert(Story story)
    {
        ArgumentNullException.ThrowIfNull(story);

        var items = story.StoryForEstimates
            .Select(e => new StoryForEstimateDto(
                e.ParticipantDisplayName,
                story.EstimateEnded ? e.Value.ToDisplayValue(story.StoryType) : e.Value.ToDisplayHasValue()))
            .ToArray();

        return new(story.Id, story.Title, story.Links.ToArray(), items, story.GetTotalValue());
    }
}