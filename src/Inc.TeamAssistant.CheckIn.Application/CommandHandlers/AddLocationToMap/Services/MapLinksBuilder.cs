using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Services;

internal sealed class MapLinksBuilder
{
    private readonly string _connectToMapLinkTemplate;

    public MapLinksBuilder(string connectToMapLinkTemplate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectToMapLinkTemplate);
        
        _connectToMapLinkTemplate = connectToMapLinkTemplate;
    }

    public string Build(LanguageId languageId, Guid mapId)
    {
        ArgumentNullException.ThrowIfNull(languageId);
        
        return string.Format(_connectToMapLinkTemplate, languageId.Value, mapId.ToString("N"));
    }
}