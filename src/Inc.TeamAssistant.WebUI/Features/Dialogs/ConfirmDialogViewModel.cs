namespace Inc.TeamAssistant.WebUI.Features.Dialogs;

public sealed record ConfirmDialogViewModel(string ConfirmText, string RejectText)
{
    public static ConfirmDialogViewModel Empty { get; } = new(string.Empty, string.Empty);
}