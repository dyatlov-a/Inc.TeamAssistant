using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.Common.Converters;

internal static class SummaryByStoryConverter
{
    public static SummaryByStory ConvertTo(AssessmentSession assessmentSession)
    {
        if (assessmentSession is null)
            throw new ArgumentNullException(nameof(assessmentSession));

        var estimateEnded = assessmentSession.EstimateEnded();

        return new SummaryByStory(
            assessmentSession.Id,
            assessmentSession.LanguageId,
            assessmentSession.ChatId,
            assessmentSession.CurrentStory.ExternalId,
            assessmentSession.CurrentStory.Title,
            assessmentSession.CurrentStory.Links,
            estimateEnded,
            assessmentSession.CurrentStory.GetTotal().ToDisplayValue(estimateEnded),
            assessmentSession.CurrentStory.StoryForEstimates
                .Select(s => new EstimateItemDetails(
                    s.Participant.Name,
                    s.Value.ToDisplayHasValue(),
                    s.Value.ToDisplayValue()))
                .ToArray());
    }
}