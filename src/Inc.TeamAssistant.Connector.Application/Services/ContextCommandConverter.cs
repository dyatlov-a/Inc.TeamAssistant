using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Languages;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class ContextCommandConverter
{
    private readonly IMessageBuilder _messageBuilder;

    public ContextCommandConverter(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }
    
    public async IAsyncEnumerable<(BotCommandScope Scope, IReadOnlyCollection<BotCommand> Commands)> Convert(
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
            if (!commandsByScopes.Any())
                continue;
            
            var botCommandScope = ToBotCommandScope(commandScope);
            var botCommands = await ToBotCommands(commandsByScopes, languageId);
            
            yield return (botCommandScope, botCommands);
        }
    }
    
    private async Task<IReadOnlyCollection<BotCommand>> ToBotCommands(
        IReadOnlyCollection<ContextCommand> contextCommands,
        LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(contextCommands);
        ArgumentNullException.ThrowIfNull(languageId);

        var result = new List<BotCommand>(contextCommands.Count);
        
        foreach (var contextCommand in contextCommands)
            result.Add(await ToBotCommand(contextCommand, languageId));

        return result;
    }

    private async Task<BotCommand> ToBotCommand(ContextCommand command, LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(languageId);
        
        return new BotCommand
        {
            Command = command.Value,
            Description = await _messageBuilder.Build(command.HelpMessageId!, languageId)
        };
    }
    
    private static BotCommandScope ToBotCommandScope(ContextScope contextScope)
    {
        return contextScope switch
        {
            ContextScope.Chats => BotCommandScope.AllGroupChats(),
            ContextScope.Private => BotCommandScope.Default(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(contextScope),
                contextScope,
                "ContextScope value was out of range.")
        };
    }
}