using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Languages;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Telegram;

internal sealed class TelegramContextCommandConverter
{
    private readonly IMessageBuilder _messageBuilder;

    public TelegramContextCommandConverter(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }
    
    public IEnumerable<(BotCommandScope Scope, IReadOnlyCollection<BotCommand> Commands)> Convert(
        IReadOnlyCollection<ContextCommand> contextCommands,
        LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(contextCommands);
        ArgumentNullException.ThrowIfNull(languageId);

        foreach (var commandScope in Enum.GetValues<ContextScope>())
        {
            var commandsByScopes = contextCommands
                .Where(c => c.HelpMessageId is not null && c.Scopes.Any(s => s == commandScope))
                .ToArray();
            
            if (commandsByScopes.Any())
            {
                var botCommandScope = ToBotCommandScope(commandScope);
                var botCommands = ToBotCommands(commandsByScopes, languageId);
            
                yield return (botCommandScope, botCommands);
            }
        }
    }
    
    private IReadOnlyCollection<BotCommand> ToBotCommands(
        IReadOnlyCollection<ContextCommand> contextCommands,
        LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(contextCommands);
        ArgumentNullException.ThrowIfNull(languageId);

        var result = contextCommands
            .Select(c => new BotCommand
            {
                Command = c.Value,
                Description = _messageBuilder.Build(c.HelpMessageId!, languageId)
            })
            .ToArray();

        return result;
    }
    
    private static BotCommandScope ToBotCommandScope(ContextScope contextScope)
    {
        return contextScope switch
        {
            ContextScope.Chats => BotCommandScope.AllGroupChats(),
            ContextScope.Private => BotCommandScope.Default(),
            _ => throw new ArgumentOutOfRangeException(nameof(contextScope), contextScope, "State out of range.")
        };
    }
}