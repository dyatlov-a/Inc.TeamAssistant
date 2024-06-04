using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public sealed record AssessmentSessionHistoryPageViewModel(
    string LinkToBackText,
    string StoryList,
    string MeanRating,
    GetStoriesResult? Data)
    : IViewModel<AssessmentSessionHistoryPageViewModel>
{
    public static AssessmentSessionHistoryPageViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        null);
}