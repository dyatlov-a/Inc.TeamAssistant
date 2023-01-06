using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Shared.Annotations;

internal sealed record MainFooterAnnotation
{
    public MarkupString Appraiser { get; }
    public MarkupString TelegramBotToEvaluateTasks { get; }
    public MarkupString EvaluateTool { get; }
    public MarkupString AboutBot { get; }
    public MarkupString Development { get; }
    public MarkupString Resources { get; }

    public MainFooterAnnotation(
        string appraiser,
        string telegramBotToEvaluateTasks,
        string evaluateTool,
        string aboutBot,
        string development,
        string resources)
    {
        Appraiser = (MarkupString)appraiser;
        TelegramBotToEvaluateTasks = (MarkupString)telegramBotToEvaluateTasks;
        EvaluateTool = (MarkupString)evaluateTool;
        AboutBot = (MarkupString)aboutBot;
        Development = (MarkupString)development;
        Resources = (MarkupString)resources;
    }

    public static readonly MainFooterAnnotation Empty = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}