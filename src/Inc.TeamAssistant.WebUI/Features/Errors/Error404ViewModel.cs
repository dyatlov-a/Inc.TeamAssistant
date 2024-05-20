using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Errors;

public sealed record Error404ViewModel(string Title, string Description)
    : IViewModel<Error404ViewModel>
{
    public static Error404ViewModel Empty { get; } = new(string.Empty, string.Empty);
}