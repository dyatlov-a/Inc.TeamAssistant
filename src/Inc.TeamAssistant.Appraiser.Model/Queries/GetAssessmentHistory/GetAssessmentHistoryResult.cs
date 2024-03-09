namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;

public sealed record GetAssessmentHistoryResult(IReadOnlyCollection<AssessmentHistoryDto> Items);