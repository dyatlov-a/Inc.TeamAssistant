using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Errors;

public sealed record Error401ViewModel(string Title, string Description)
    : IViewModel<Error401ViewModel>
{
    public static Error401ViewModel Empty { get; } = new(string.Empty, string.Empty);
}