namespace Inc.TeamAssistant.Primitives;

public sealed record Person(long Id, string Name, string? Username)
{
    public string DisplayName => Username?.Equals(Name, StringComparison.InvariantCultureIgnoreCase) == false
        ? $"{Name} ({Username})"
        : Name;
}