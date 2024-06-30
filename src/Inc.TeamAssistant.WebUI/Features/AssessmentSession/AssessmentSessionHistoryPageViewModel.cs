using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public sealed record AssessmentSessionHistoryPageViewModel(
    string StoryList,
    string MeanRating,
    IReadOnlyCollection<BreadcrumbItem> Items,
    GetStoriesResult? Data)
    : IViewModel<AssessmentSessionHistoryPageViewModel>
{
    public static AssessmentSessionHistoryPageViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        Array.Empty<BreadcrumbItem>(),
        null);
}