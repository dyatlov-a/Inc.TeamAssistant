namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed record StagesPageViewModel(string Title, StagesState StagesState)
{
    public static readonly StagesPageViewModel Empty = new(string.Empty, StagesState.Empty);
}