using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Components.Annotations;

internal sealed record FeedbackAnnotation
{
    public MarkupString AskQuestion { get; }
    public MarkupString WhatYouThink { get; }
    public MarkupString FeedbackSubject { get; }
    public MarkupString FeedbackQuestion { get; }
    public MarkupString FeedbackSend { get; }

    public FeedbackAnnotation(
        string askQuestion,
        string whatYouThink,
        string feedbackSubject,
        string feedbackQuestion,
        string feedbackSend)
    {
        AskQuestion = (MarkupString)askQuestion;
        WhatYouThink = (MarkupString)whatYouThink;
        FeedbackSubject = (MarkupString)feedbackSubject;
        FeedbackQuestion = (MarkupString)feedbackQuestion;
        FeedbackSend = (MarkupString)feedbackSend;
    }

    public static readonly FeedbackAnnotation Empty = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}