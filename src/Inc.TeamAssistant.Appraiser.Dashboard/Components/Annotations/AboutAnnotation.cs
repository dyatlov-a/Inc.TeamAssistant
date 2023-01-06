using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Components.Annotations;

internal sealed record AboutAnnotation
{
    public MarkupString Appraiser { get; }
    public MarkupString TelegramBotToEvaluateTasks { get; }
    public MarkupString EvaluateTool { get; }
    public MarkupString Scan { get; }
    public MarkupString ToStart { get; }

    public AboutAnnotation(
        string appraiser,
        string telegramBotToEvaluateTasks,
        string evaluateTool,
        string scan,
        string toStart)
    {
        Appraiser = (MarkupString)appraiser;
        TelegramBotToEvaluateTasks = (MarkupString)telegramBotToEvaluateTasks;
        EvaluateTool = (MarkupString)evaluateTool;
        Scan = (MarkupString)scan;
        ToStart = (MarkupString)toStart;
    }

    public static readonly AboutAnnotation Empty = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}