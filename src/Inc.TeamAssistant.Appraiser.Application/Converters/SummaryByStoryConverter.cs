using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.Converters;

internal static class SummaryByStoryConverter
{
    public static SummaryByStory ConvertTo(Story story)
    {
        ArgumentNullException.ThrowIfNull(story);

        var storyForEstimates = story.StoryForEstimates
            .Select(s => new EstimateItemDetails(
                s.ParticipantDisplayName,
                s.Value.ToDisplayHasValue(),
                s.Value.ToDisplayValue(story.StoryType)))
            .ToArray();
        var assessments = story.GetAssessments()
            .Select(a => a.ToString())
            .ToArray();

        return new SummaryByStory(
            story.TeamId,
            story.LanguageId,
            story.ChatId,
            story.Id,
            story.ExternalId,
            story.Title,
            story.Links.ToArray(),
            story.EstimateEnded,
            story.GetTotalValue(),
            storyForEstimates,
            assessments,
            story.Accepted);
    }
}