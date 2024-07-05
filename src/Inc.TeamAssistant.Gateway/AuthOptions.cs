using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway;

public sealed class AuthOptions
{
    public string LinkForConnectTemplate { get; set; } = default!;
    public Guid BotId { get; set; }
    public Person SuperUser { get; set; } = default!;
}