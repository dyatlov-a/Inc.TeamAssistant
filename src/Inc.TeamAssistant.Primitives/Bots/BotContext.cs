namespace Inc.TeamAssistant.Primitives.Bots;

public sealed record BotContext(Guid Id, string UserName, IReadOnlyDictionary<string, string> Properties)
{
    public TimeSpan GetIntervalOrDefault(string propertyKey, TimeSpan defaultValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyKey);
        
        var interval = Properties.GetValueOrDefault(propertyKey);
        if (string.IsNullOrWhiteSpace(interval))
            return defaultValue;
        
        return TimeSpan.TryParse(interval, out var intervalValue)
            ? intervalValue
            : defaultValue;
    }
}