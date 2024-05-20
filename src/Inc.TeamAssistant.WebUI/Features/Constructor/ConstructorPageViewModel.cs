using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor;

public sealed record ConstructorPageViewModel(string Title, string SelectBotText)
    : IViewModel<ConstructorPageViewModel>
{
    public static ConstructorPageViewModel Empty { get; } = new(string.Empty, string.Empty);
}