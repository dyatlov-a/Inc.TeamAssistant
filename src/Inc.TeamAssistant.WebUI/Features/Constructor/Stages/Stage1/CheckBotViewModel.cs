using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage1;

public sealed record CheckBotViewModel(
    string FormSectionTokenTitle,
    string FormSectionTokenHelpTemplate,
    string FormSectionTokenFieldTokenLabel,
    string FormSectionTokenFieldUserNameLabel,
    string MoveNextTitle)
    : IViewModel<CheckBotViewModel>
{
    public static CheckBotViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}