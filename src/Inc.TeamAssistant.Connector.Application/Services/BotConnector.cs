using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotConnector : IBotConnector
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly IBotReader _botReader;

    public BotConnector(IMessageBuilder messageBuilder, IBotReader botReader)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
    }
    
    public async Task<string?> GetUsername(string botToken, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(botToken);
        
        try
        {
            var client = new TelegramBotClient(botToken);
            
            var bot = await client.GetMeAsync(token);

            return bot.Username;
        }
        catch
        {
            return null;
        }
    }

    public async Task Setup(Guid botId, CancellationToken token)
    {
        var bot = await _botReader.Find(botId, DateTimeOffset.UtcNow, token);
        if (bot is null)
            throw new TeamAssistantUserException(Messages.Connector_BotNotFound, botId);
        
        var client = new TelegramBotClient(bot.Token);
        
        await SetDefaultAdministratorRights(client, token);

        foreach (var languageId in LanguageSettings.LanguageIds)
        {
            await SetCommands(client, bot, languageId, token);
            await SetShortDescription(client, languageId, token);
            await SetDescription(client, languageId, token);
        }
    }

    private async Task SetDefaultAdministratorRights(ITelegramBotClient client, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        
        var rights = new ChatAdministratorRights
        {
            CanDeleteMessages = true,
            CanPinMessages = true,
            CanManageChat = true
        };
        
        await client.SetMyDefaultAdministratorRightsAsync(rights, cancellationToken: token);
    }

    private async Task SetCommands(ITelegramBotClient client, Bot bot, LanguageId languageId, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(languageId);

        foreach (var commandScope in Enum.GetValues<CommandScope>())
            if (TryConvert(commandScope, out var scope))
            {
                var commandsByScopes = (await Build(bot.Commands, commandScope, languageId))
                    .OrderBy(c => c.Command)
                    .ToArray();

                await client.SetMyCommandsAsync(
                    commandsByScopes,
                    scope,
                    languageCode: languageId.Value,
                    cancellationToken: token);
            }
    }

    private async Task SetShortDescription(ITelegramBotClient client, LanguageId languageId, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(languageId);
        
        var shortDescription = await _messageBuilder.Build(Messages.Connector_BotShortDescription, languageId);
        
        await client.SetMyShortDescriptionAsync(shortDescription, languageId.Value, token);
    }
    
    private async Task SetDescription(ITelegramBotClient client, LanguageId languageId, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(languageId);
        
        var description = await _messageBuilder.Build(Messages.Connector_BotDescription, languageId);
        
        await client.SetMyDescriptionAsync(description, languageId.Value, token);
    }
    
    private async Task<IReadOnlyCollection<Telegram.Bot.Types.BotCommand>> Build(
        IEnumerable<Domain.BotCommand> botCommands,
        CommandScope commandScope,
        LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(botCommands);
        ArgumentNullException.ThrowIfNull(languageId);

        var commands = new List<Telegram.Bot.Types.BotCommand>();

        foreach (var botCommand in botCommands)
            if (botCommand.HelpMessageId is not null && botCommand.Scopes.Any(s => s == commandScope))
                commands.Add(new Telegram.Bot.Types.BotCommand
                {
                    Command = botCommand.Value,
                    Description = await _messageBuilder.Build(botCommand.HelpMessageId, languageId)
                });
        
        return commands;
    }
    
    private static bool TryConvert(CommandScope commandScope, out BotCommandScope? botCommandScope)
    {
        botCommandScope = commandScope switch
        {
            CommandScope.AllGroupChats => BotCommandScope.AllGroupChats(),
            CommandScope.Default => BotCommandScope.Default(),
            _ => null
        };
        
        return botCommandScope is not null;
    }
}