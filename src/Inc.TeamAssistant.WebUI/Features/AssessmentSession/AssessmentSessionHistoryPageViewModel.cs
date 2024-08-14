using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public sealed record AssessmentSessionHistoryPageViewModel(
    string StoryList,
    string MeanRating,
    IReadOnlyCollection<BreadcrumbItem> Breadcrumbs,
    IReadOnlyCollection<StoryDto> Items)
    : IViewModel<AssessmentSessionHistoryPageViewModel>
{
    public static AssessmentSessionHistoryPageViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        Array.Empty<BreadcrumbItem>(),
        Array.Empty<StoryDto>());
}