namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public static class BotPropertiesBuilder
{
    private static readonly IReadOnlyDictionary<string, string> Defaults = new Dictionary<string, string>
    {
        ["roundInterval"] = "14.00:00:00",
        ["votingInterval"] = "1.00:00:00",
        ["nextReviewerStrategy"] = "RoundRobin",
        ["waitingNotificationInterval"] = "00:30:00",
        ["inProgressNotificationInterval"] = "01:00:00",
        ["storyType"] = "Fibonacci"
    };

    public static IEnumerable<KeyValuePair<string, string>> Build(IReadOnlyDictionary<string, string> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        
        foreach (var key in Defaults.Keys)
        {
            var value = properties.TryGetValue(key, out var propertyValue)
                ? propertyValue
                : Defaults[key];

            yield return new KeyValuePair<string, string>(key, value);
        }
    }
}