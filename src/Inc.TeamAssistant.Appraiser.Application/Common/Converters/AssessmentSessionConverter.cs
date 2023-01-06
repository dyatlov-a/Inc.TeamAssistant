using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.Common.Converters;

internal static class AssessmentSessionConverter
{
    public static AssessmentSessionDetails ConvertTo(AssessmentSession assessmentSession)
    {
        if (assessmentSession is null)
            throw new ArgumentNullException(nameof(assessmentSession));

        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(s => new EstimateItemDetails(
                s.Participant.Id,
                s.Participant.Name,
                s.StoryExternalId,
                s.Value.ToDisplayHasValue(),
                s.Value.ToDisplayValue()))
            .ToArray();

        return new(
            assessmentSession.Id,
            assessmentSession.Title,
            assessmentSession.LanguageId,
            StoryConverter.ConvertTo(assessmentSession.CurrentStory),
            items);
    }
}