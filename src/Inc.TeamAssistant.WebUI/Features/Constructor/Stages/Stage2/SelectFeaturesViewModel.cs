using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage2;

public sealed record SelectFeaturesViewModel(
    string FormSectionFeaturesTitle,
    string FormSectionFeaturesHelp,
    string FormSectionFeaturesAddText,
    string FormSectionFeaturesRemoveText,
    string FormSectionFeaturesAvailableEmptyText,
    string FormSectionFeaturesSelectedEmptyText,
    string MoveNextTitle,
    Dictionary<string, string> Features)
    : IViewModel<SelectFeaturesViewModel>
{
    public static SelectFeaturesViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        new Dictionary<string, string>());
}