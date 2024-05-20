using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway;

public sealed class AuthOptions
{
    public Guid BotId { get; set; }
    public Person SuperUser { get; set; } = default!;
}