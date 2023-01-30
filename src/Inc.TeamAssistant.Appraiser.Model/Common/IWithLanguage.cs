using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Common;

public interface IWithLanguage
{
    LanguageId LanguageId { get; }
}