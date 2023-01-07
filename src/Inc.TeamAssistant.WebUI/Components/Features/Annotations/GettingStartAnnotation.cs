using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Components.Features.Annotations;

internal sealed record GettingStartAnnotation
{
    public MarkupString EasyStart { get; }
    public MarkupString StepsToStart { get; }
    public MarkupString StepCreateChat { get; }
    public MarkupString StepInviteMembers { get; }
    public MarkupString StepCreateAssessmentSession { get; }
    public MarkupString StepFollowTheLink { get; }
    public MarkupString StepAddTask { get; }

    public GettingStartAnnotation(
        string easyStart,
        string stepsToStart,
        string stepCreateChat,
        string stepInviteMembers,
        string stepCreateAssessmentSession,
        string stepFollowTheLink,
        string stepAddTask)
    {
        EasyStart = (MarkupString)easyStart;
        StepsToStart = (MarkupString)stepsToStart;
        StepCreateChat = (MarkupString)stepCreateChat;
        StepInviteMembers = (MarkupString)stepInviteMembers;
        StepCreateAssessmentSession = (MarkupString)stepCreateAssessmentSession;
        StepFollowTheLink = (MarkupString)stepFollowTheLink;
        StepAddTask = (MarkupString)stepAddTask;
    }

    public static readonly GettingStartAnnotation Empty = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}