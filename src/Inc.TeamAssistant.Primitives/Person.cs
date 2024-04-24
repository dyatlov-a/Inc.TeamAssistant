namespace Inc.TeamAssistant.Primitives;

public sealed record Person(long Id, string Name, string? Username)
{
    public static readonly Person Empty = new(0, string.Empty, null);
    
    public string DisplayName => Username?.Equals(Name, StringComparison.InvariantCultureIgnoreCase) == false
        ? $"{Name} ({Username})"
        : Name;
}