using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed record CompleteViewModel(
    string FormSectionTokenTitle,
    string FormSectionTokenHelp,
    string FormSectionTokenFieldUserNameLabel,
    string FormSectionFeaturesTitle,
    string FormSectionFeaturesHelp,
    IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> SettingSections,
    string ButtonCreateText,
    string ButtonUpdateText,
    string BooleanTrueText,
    string BooleanFalseText,
    string EditText,
    IReadOnlyDictionary<string, string> FeaturesLookup,
    string BotInformation,
    string BotInformationHelp,
    string BotName,
    string BotShortDescription,
    string BotDescription)
    : IViewModel<CompleteViewModel>
{
    public static CompleteViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        new Dictionary<string, IReadOnlyCollection<SettingSection>>(),
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        new Dictionary<string, string>(),
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}