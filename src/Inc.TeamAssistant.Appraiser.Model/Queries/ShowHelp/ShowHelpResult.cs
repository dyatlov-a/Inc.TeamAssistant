using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;

public sealed record ShowHelpResult(LanguageId LanguageId, IReadOnlyCollection<string> CommandsHelp);