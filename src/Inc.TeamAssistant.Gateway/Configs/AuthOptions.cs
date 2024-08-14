using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Configs;

public sealed class AuthOptions
{
    public string LinkForConnectTemplate { get; set; } = default!;
    public Guid BotId { get; set; }
    public Person? SuperUser { get; set; }
}