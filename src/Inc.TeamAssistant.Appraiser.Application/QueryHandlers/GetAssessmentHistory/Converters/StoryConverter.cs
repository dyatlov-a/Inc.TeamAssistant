using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetAssessmentHistory.Converters;

public static class AssessmentHistoryDtoConverter
{
    public static IReadOnlyCollection<AssessmentHistoryDto> ConvertFrom(IReadOnlyCollection<Story> stories, int limit)
    {
        ArgumentNullException.ThrowIfNull(stories);
        
        var results = stories
            .GroupBy(h => new DateOnly(h.Created.Year, h.Created.Month, h.Created.Day))
            .Select(h => new AssessmentHistoryDto(h.Key, h.Count(), h.Sum(s => s.GetWeight())))
            .OrderByDescending(h => h.AssessmentDate)
            .Take(limit)
            .ToArray();

        return results;
    }
}