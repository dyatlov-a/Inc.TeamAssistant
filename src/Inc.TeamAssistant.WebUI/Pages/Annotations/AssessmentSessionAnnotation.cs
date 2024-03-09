using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Pages.Annotations;

internal sealed record AssessmentSessionAnnotation
{
    public MarkupString TaskAssess { get; }
    public MarkupString CardNotSelected { get; }
    public MarkupString Scan { get; }
    public MarkupString ToConnect { get; }
    public MarkupString AssessmentSessionCard { get; }
    public MarkupString AssessmentSessionNotFound { get; }
    public MarkupString Loading { get; }

    public AssessmentSessionAnnotation(
        string taskAssess,
        string cardNotSelected,
        string scan,
        string toConnect,
        string assessmentSessionCard,
        string assessmentSessionNotFound,
        string loading)
    {
        TaskAssess = (MarkupString)taskAssess;
        CardNotSelected = (MarkupString)cardNotSelected;
        Scan = (MarkupString)scan;
        ToConnect = (MarkupString)toConnect;
        AssessmentSessionCard = (MarkupString)assessmentSessionCard;
        AssessmentSessionNotFound = (MarkupString)assessmentSessionNotFound;
        Loading = (MarkupString)loading;
    }
    
    public static readonly AssessmentSessionAnnotation Empty = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}