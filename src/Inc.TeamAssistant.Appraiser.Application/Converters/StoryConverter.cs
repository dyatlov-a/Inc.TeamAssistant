using Inc.TeamAssistant.Appraiser.Application.Extensions;
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
            .Select(e => new StoryForEstimateDto(
                e.ParticipantId,
                e.ParticipantDisplayName,
                story.EstimateEnded ? e.Value.ToDisplayValue(story.StoryType) : e.Value.ToDisplayHasValue(),
                e.Value == AssessmentValue.Value.None ? null : (int)e.Value))
            .ToArray();

        return new(
            story.Id,
            story.Title,
            story.Links.ToArray(),
            items,
            story.EstimateEnded,
            story.GetTotalValue(),
            HasMedian: story.StoryType == StoryType.Scrum,
            story.CalculateMedian());
    }
}