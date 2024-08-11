namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;

public sealed record AssessmentHistoryDto(DateOnly AssessmentDate, int StoriesCount, int AssessmentSum);