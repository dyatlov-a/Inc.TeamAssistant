using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Features.Dialogs;

public sealed record AcceptCookieDialogViewModel
{
    public MarkupString Message { get; }
    public MarkupString ButtonText { get; }

    public AcceptCookieDialogViewModel(string message, string buttonText)
    {
        Message = (MarkupString)message;
        ButtonText = (MarkupString)buttonText;
    }
    
    public static readonly AcceptCookieDialogViewModel Empty = new(string.Empty, string.Empty);
}