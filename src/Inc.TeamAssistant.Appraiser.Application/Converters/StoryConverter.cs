using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.Converters;

internal static class StoryConverter
{
    public static StoryDto Convert(Story story)
    {
        ArgumentNullException.ThrowIfNull(story);

        var items = story.StoryForEstimates
            .OrderByDescending(e => e.Value)
            .ThenBy(e => e.ParticipantDisplayName)
            .Select(e =>
            {
                var estimation = story.ToEstimation(e.Value);

                return new StoryForEstimateDto(
                    e.ParticipantId,
                    e.ParticipantDisplayName,
                    $"/photos/{e.ParticipantId}",
                    story.EstimateEnded ? estimation.DisplayValue : estimation.HasValue,
                    e.Value == Estimation.None.Value ? null : e.Value);
            })
            .ToArray();

        return new(
            story.Id,
            story.Title,
            story.Links.ToArray(),
            items,
            story.EstimateEnded,
            story.CalculateMean().DisplayValue,
            story.CalculateMedian().DisplayValue,
            story.AcceptedValue.DisplayValue);
    }
}