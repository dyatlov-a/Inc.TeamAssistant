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
                var estimation = story.ToEstimation(s.Value);
                return new EstimateItemDetails(
                    s.ParticipantDisplayName,
                    estimation.HasValue,
                    estimation.DisplayValue);
            })
            .ToArray();
        var assessments = story.GetAssessments()
            .Select(s => new EstimateDto(s.Code, s.Value))
            .ToArray();
        var assessmentsToAccept = story.GetTopValues()
            .OrderBy(s => s.Value)
            .Select(s => new EstimateDto(s.Code, s.Value))
            .ToArray();

        return new SummaryByStory(
            story.TeamId,
            story.LanguageId,
            story.ChatId,
            story.Id,
            story.ExternalId,
            story.Title,
            story.StoryType.ToString(),
            story.Links.ToArray(),
            story.EstimateEnded,
            story.CalculateMean().DisplayValue,
            story.CalculateMedian().DisplayValue,
            story.AcceptedValue.DisplayValue,
            storyForEstimates,
            assessments,
            story.Accepted,
            assessmentsToAccept);
    }
}