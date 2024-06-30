using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Auth;

public sealed record AuthorizingDetailsViewModel(string Description)
    : IViewModel<AuthorizingDetailsViewModel>
{
    public static AuthorizingDetailsViewModel Empty { get; } = new(string.Empty);
}