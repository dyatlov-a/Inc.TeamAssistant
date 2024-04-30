using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public sealed record AssessmentSessionHistoryViewModel(
    string Title,
    string TasksName,
    ServiceResult<GetAssessmentHistoryResult?> Data)
{
    public static readonly AssessmentSessionHistoryViewModel Empty = new(
        string.Empty,
        string.Empty,
        ServiceResult<GetAssessmentHistoryResult?>.Empty);
}