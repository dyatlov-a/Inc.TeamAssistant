using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dialogs;

public sealed record AcceptCookieDialogViewModel(string Message, string ButtonText)
    : IViewModel<AcceptCookieDialogViewModel>
{
    public static AcceptCookieDialogViewModel Empty { get; } = new(string.Empty, string.Empty);
}