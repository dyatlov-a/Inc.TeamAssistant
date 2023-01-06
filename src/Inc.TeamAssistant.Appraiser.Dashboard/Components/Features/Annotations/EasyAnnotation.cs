using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Components.Features.Annotations;

internal sealed record EasyAnnotation
{
    public MarkupString ComfortableInUsing { get; }
    public MarkupString InteractionOptions { get; }
    public MarkupString InteractionOptionRestartVoting { get; }
    public MarkupString TerminationOfVoting { get; }
    public MarkupString ListOfConnected { get; }

    public EasyAnnotation(
        string comfortableInUsing,
        string interactionOptions,
        string interactionOptionRestartVoting,
        string terminationOfVoting,
        string listOfConnected)
    {
        ComfortableInUsing = (MarkupString)comfortableInUsing;
        InteractionOptions = (MarkupString)interactionOptions;
        InteractionOptionRestartVoting = (MarkupString)interactionOptionRestartVoting;
        TerminationOfVoting = (MarkupString)terminationOfVoting;
        ListOfConnected = (MarkupString)listOfConnected;
    }

    public static readonly EasyAnnotation Empty = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}