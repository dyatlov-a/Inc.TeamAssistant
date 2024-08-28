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
                var value = e.GetValue(story.StoryType);

                return new StoryForEstimateDto(
                    e.ParticipantId,
                    e.ParticipantDisplayName,
                    story.EstimateEnded ? value.DisplayValue : value.HasValue,
                    e.Value == Estimation.None ? null : e.Value);
            })
            .ToArray();

        return new(
            story.Id,
            story.Title,
            story.Links.ToArray(),
            items,
            story.EstimateEnded,
            EstimationProvider.GetTotalValue(story),
            HasMedian: story.StoryType == StoryType.Scrum,
            EstimationProvider.CalculateMedian(story));
    }
}