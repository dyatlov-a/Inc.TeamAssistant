using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.Converters;

internal static class SummaryByStoryConverter
{
    public static SummaryByStory ConvertTo(Story story)
    {
        ArgumentNullException.ThrowIfNull(story);

        var storyForEstimates = story.StoryForEstimates
            .Select(s =>
            {
                var value = s.GetValue(story.StoryType);
                return new EstimateItemDetails(s.ParticipantDisplayName, value.HasValue, value.DisplayValue);
            })
            .ToArray();
        var assessments = story.GetAssessments()
            .Select(s => s.Value)
            .ToArray();
        var assessmentsToAccept = story.GetTopValues()
            .OrderBy(s => s)
            .Select(s => s.ToString())
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
            EstimationProvider.GetTotalValue(story),
            EstimationProvider.GetAcceptedValue(story),
            storyForEstimates,
            assessments,
            story.Accepted,
            assessmentsToAccept);
    }
}