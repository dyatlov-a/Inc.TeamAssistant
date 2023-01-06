using System.Text.Json;
using Inc.TeamAssistant.Appraiser.Dashboard;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Backend.Services.MessageProviders;

internal sealed class MessageProvider : IMessageProvider
{
    private readonly string _webRootPath;

    public MessageProvider(string webRootPath)
    {
        if (string.IsNullOrWhiteSpace(webRootPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(webRootPath));

        _webRootPath = webRootPath;
    }

    public async Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> Get()
    {
        var result = new Dictionary<string, Dictionary<string, string>>();

        foreach (var languageId in Settings.LanguageIds)
        {
            var language = Path.Combine(_webRootPath, "langs", $"{languageId.Value}.json");
            var resourcesAsString = await File.ReadAllTextAsync(language);
            var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(resourcesAsString);

            result.Add(languageId.Value, resources!);
        }

        return ServiceResult.Success(result);
    }
}