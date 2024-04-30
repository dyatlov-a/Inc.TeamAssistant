namespace Inc.TeamAssistant.WebUI.Features.Errors;

internal sealed record Error404ViewModel(string Title, string Description)
{
    public static readonly Error404ViewModel Empty = new(string.Empty, string.Empty);
}