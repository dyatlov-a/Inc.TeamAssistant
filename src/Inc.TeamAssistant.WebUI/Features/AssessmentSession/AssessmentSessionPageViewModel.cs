using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public sealed record AssessmentSessionPageViewModel(
    string TaskAssess,
    string CardNotSelected,
    string Scan,
    string ToConnect,
    string AssessmentSessionCard,
    string AssessmentSessionNotFound,
    string Loading,
    string AverageRating,
    ServiceResult<GetStoryDetailsResult?> Data)
    : IViewModel<AssessmentSessionPageViewModel>
{
    public static AssessmentSessionPageViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        ServiceResult<GetStoryDetailsResult?>.Empty);
    
    public string StateMessage => Data.State switch
    {
        ServiceResultState.NotFound => AssessmentSessionNotFound,
        ServiceResultState.Failed => Data.ErrorMessage,
        _ => Loading
    };
}