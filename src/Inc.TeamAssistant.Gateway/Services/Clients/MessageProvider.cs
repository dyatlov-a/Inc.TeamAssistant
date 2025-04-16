using System.Text.Json;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class MessageProvider : IMessageProvider
{
    private readonly string _webRootPath;

    public MessageProvider(string webRootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(webRootPath);

        _webRootPath = webRootPath;
    }

    public Dictionary<string, Dictionary<string, string>> Get()
    {
        var result = new Dictionary<string, Dictionary<string, string>>();

        foreach (var languageId in LanguageSettings.LanguageIds)
        {
            var language = Path.Combine(_webRootPath, "langs", $"{languageId.Value}.json");
            var resourcesAsString = File.ReadAllText(language);
            var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(resourcesAsString);

            result.Add(languageId.Value, resources!);
        }

        return result;
    }
}