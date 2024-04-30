using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public sealed record AssessmentSessionHistoryPageViewModel(
    string LinkToBackText,
    string StoryList,
    ServiceResult<GetStoriesResult?> Data)
{
    public static readonly AssessmentSessionHistoryPageViewModel Empty = new(
        string.Empty,
        string.Empty,
        ServiceResult<GetStoriesResult?>.Empty);
}