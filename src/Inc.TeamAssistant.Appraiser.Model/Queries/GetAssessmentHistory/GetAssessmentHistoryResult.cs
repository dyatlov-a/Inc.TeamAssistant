using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;

public sealed record GetAssessmentHistoryResult(IReadOnlyCollection<AssessmentHistoryDto> Items)
    : IWithEmpty<GetAssessmentHistoryResult>
{
    public static GetAssessmentHistoryResult Empty { get; } = new(Array.Empty<AssessmentHistoryDto>());
}