using System.Text.Json;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class MessageDataBuilder : IMessageProvider
{
    public MessageDataBuilder(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
    
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Data { get; }

    public static IMessageProvider Build(string webRootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(webRootPath);
        
        var data = new Dictionary<string, IReadOnlyDictionary<string, string>>();

        foreach (var languageId in LanguageSettings.LanguageIds)
        {
            var language = Path.Combine(webRootPath, "langs", $"{languageId.Value}.json");
            var resourcesAsString = File.ReadAllText(language);
            var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(resourcesAsString);

            data.Add(languageId.Value, resources!);
        }

        return new MessageDataBuilder(data);
    }
}