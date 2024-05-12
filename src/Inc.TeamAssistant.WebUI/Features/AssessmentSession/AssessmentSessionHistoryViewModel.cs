using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public sealed record AssessmentSessionHistoryViewModel(
    string Title,
    string TasksName,
    ServiceResult<GetAssessmentHistoryResult?> Data)
    : IViewModel<AssessmentSessionHistoryViewModel>
{
    public static AssessmentSessionHistoryViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        ServiceResult<GetAssessmentHistoryResult?>.Empty);
}