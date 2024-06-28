using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

public sealed record AssessmentSessionPageViewModel(
    string TaskAssess,
    string MeanRating,
    string MedianRating,
    string AboutTemplate,
    string ConnectToTeam,
    string HasNotTasks,
    string TeamName,
    string CodeForConnect,
    IReadOnlyCollection<BreadcrumbItem> Items,
    StoryDto? Story)
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
        Array.Empty<BreadcrumbItem>(),
        null);
}