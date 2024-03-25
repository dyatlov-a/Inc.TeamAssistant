namespace Inc.TeamAssistant.Primitives;

public sealed record Person(long Id, string Name, string? Username)
{
    public string DisplayName => string.IsNullOrWhiteSpace(Username) ? Name : $"{Name} ({Username})";
}