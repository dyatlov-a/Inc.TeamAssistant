using System.Text.Json;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class MessageProvider : IMessageProvider
{
    private readonly string _webRootPath;

    public MessageProvider(string webRootPath)
    {
        if (string.IsNullOrWhiteSpace(webRootPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(webRootPath));

        _webRootPath = webRootPath;
    }

    public async Task<Dictionary<string, Dictionary<string, string>>> Get(CancellationToken token)
    {
        var result = new Dictionary<string, Dictionary<string, string>>();

        foreach (var languageId in LanguageSettings.LanguageIds)
        {
            var language = Path.Combine(_webRootPath, "langs", $"{languageId.Value}.json");
            var resourcesAsString = await File.ReadAllTextAsync(language, token);
            var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(resourcesAsString);

            result.Add(languageId.Value, resources!);
        }

        return result;
    }
}