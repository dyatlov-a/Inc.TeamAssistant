namespace Inc.TeamAssistant.WebUI.Features.Dialogs;

public sealed record AcceptCookieDialogViewModel(string Message, string ButtonText)
{
    public static readonly AcceptCookieDialogViewModel Empty = new(string.Empty, string.Empty);
}