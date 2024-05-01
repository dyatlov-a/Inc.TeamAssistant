using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

internal sealed record AssessmentSessionPageViewModel(
    string TaskAssess,
    string CardNotSelected,
    string Scan,
    string ToConnect,
    string AssessmentSessionCard,
    string AssessmentSessionNotFound,
    string Loading,
    ServiceResult<GetStoryDetailsResult?> Data)
{
    public static readonly AssessmentSessionPageViewModel Empty = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        ServiceResult<GetStoryDetailsResult?>.Empty);
}