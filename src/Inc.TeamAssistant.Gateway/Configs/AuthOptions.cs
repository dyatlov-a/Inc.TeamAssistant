using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Configs;

public sealed class AuthOptions
{
    public Guid BotId { get; set; }
    public IReadOnlyCollection<Person> SystemUsers { get; set; } = [];
}