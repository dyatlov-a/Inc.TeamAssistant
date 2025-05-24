using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Features.Auth;

public sealed record AuthBotContext(string UserName, IReadOnlyCollection<Person> SystemUsers)
    : IWithEmpty<AuthBotContext>
{
    public static AuthBotContext Empty { get; } = new(string.Empty, []);
}