using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.Common.Converters;

internal static class SummaryByStoryConverter
{
    public static SummaryByStory ConvertTo(Story story)
    {
        if (story is null)
            throw new ArgumentNullException(nameof(story));

        var estimateEnded = story.EstimateEnded();

        return new SummaryByStory(
            story.TeamId,
            story.LanguageId,
            story.ChatId,
            story.Id,
            story.ExternalId,
            story.Title,
            story.Links.ToArray(),
            estimateEnded,
            story.GetTotal().ToDisplayValue(estimateEnded),
            story.StoryForEstimates
                .Select(s => new EstimateItemDetails(
                    s.ParticipantDisplayName,
                    s.Value.ToDisplayHasValue(),
                    s.Value.ToDisplayValue()))
                .ToArray());
    }
}