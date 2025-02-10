namespace Inc.TeamAssistant.Primitives.Bots;

public sealed record BotContext(Guid Id, string UserName, IReadOnlyDictionary<string, string> Properties)
    : IWithEmpty<BotContext>
{
    public static BotContext Empty { get; } = new(Guid.Empty, string.Empty, new Dictionary<string, string>());

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