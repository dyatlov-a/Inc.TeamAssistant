using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Pages.Annotations;

internal sealed record AssessmentSessionAnnotation
{
    private readonly string _taskAssessTemplate;

    public MarkupString CardNotSelected { get; }
    public MarkupString AverageRating { get; }
    public MarkupString Scan { get; }
    public MarkupString ToConnect { get; }
    public MarkupString AssessmentSessionCard { get; }
    public MarkupString AssessmentSessionNotFound { get; }
    public MarkupString Loading { get; }

    public AssessmentSessionAnnotation(
        string taskAssessTemplate,
        string cardNotSelected,
        string averageRating,
        string scan,
        string toConnect,
        string assessmentSessionCard,
        string assessmentSessionNotFound,
        string loading)
    {
        _taskAssessTemplate = taskAssessTemplate;
        CardNotSelected = (MarkupString)cardNotSelected;
        AverageRating = (MarkupString)averageRating;
        Scan = (MarkupString)scan;
        ToConnect = (MarkupString)toConnect;
        AssessmentSessionCard = (MarkupString)assessmentSessionCard;
        AssessmentSessionNotFound = (MarkupString)assessmentSessionNotFound;
        Loading = (MarkupString)loading;
    }

    public MarkupString GetTitle(string storyTitle) => (MarkupString)string.Format(_taskAssessTemplate, storyTitle);

    public static readonly AssessmentSessionAnnotation Empty = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}