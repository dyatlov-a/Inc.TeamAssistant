namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public static class BotProperties
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

    public static IReadOnlyDictionary<string, string> AddDefaults(this IReadOnlyDictionary<string, string> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        var results = new Dictionary<string, string>();
        
        foreach (var key in Defaults.Keys)
        {
            var value = properties.TryGetValue(key, out var botValue)
                ? botValue
                : Defaults[key];

            results.Add(key, value);
        }

        return results;
    }
}