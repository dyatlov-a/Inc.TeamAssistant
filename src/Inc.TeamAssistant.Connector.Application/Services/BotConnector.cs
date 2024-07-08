using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Extensions;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotConnector : IBotConnector
{
    private static readonly IReadOnlyDictionary<BotCommandScope, IReadOnlyCollection<string>> CommandsByScopes =
        new Dictionary<BotCommandScope, IReadOnlyCollection<string>>
        {
            [BotCommandScope.AllGroupChats()] = new[]
            {
                "/location",
                "/invite",
                "/new_team",
                "/leave_team",
                "/cancel",
                "/add",
                "/move_to_sp",
                "/move_to_tshirts",
                "/need_review",
                "/change_to_round_robin",
                "/change_to_random",
                "/remove_team",
                "/help"
            },
            [BotCommandScope.Default()] = new[]
            {
                "/leave_team",
                "/cancel",
                "/add",
                "/move_to_sp",
                "/move_to_tshirts",
                "/need_review",
                "/change_to_round_robin",
                "/change_to_random",
                "/remove_team",
                "/help"
            }
        };
    
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
        
        await EnsureDefaultAdministratorRights(client, token);

        foreach (var languageId in LanguageSettings.LanguageIds)
        {
            await EnsureCommands(client, bot, languageId, token);
            await EnsureShortDescription(client, languageId, token);
            await EnsureDescription(client, languageId, token);
        }
    }

    private async Task EnsureDefaultAdministratorRights(ITelegramBotClient client, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        
        var rights = new ChatAdministratorRights
        {
            CanDeleteMessages = true,
            CanPinMessages = true,
            CanManageChat = true
        };
        
        var currentRights = await client.GetMyDefaultAdministratorRightsAsync(cancellationToken: token);

        if (rights.Unwrap().Zip(currentRights.Unwrap()).Any(r => r.First != r.Second))
            await client.SetMyDefaultAdministratorRightsAsync(rights, cancellationToken: token);
    }

    private async Task EnsureCommands(
        ITelegramBotClient client,
        Bot bot,
        LanguageId languageId,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(languageId);
        
        var allBotCommands = (await Convert(bot.Commands, languageId)).ToDictionary(
            c => c.Command,
            StringComparer.InvariantCultureIgnoreCase);

        foreach (var commandsByScope in CommandsByScopes)
        {
            var botCommandsByScope = commandsByScope.Value
                .Where(c => allBotCommands.ContainsKey(c))
                .Select(c => allBotCommands[c])
                .OrderBy(c => c.Command)
                .ToArray();
            var currentBotCommandsByScope = (await client.GetMyCommandsAsync(commandsByScope.Key, languageId.Value, token))
                .OrderBy(c => c.Command)
                .ToArray();

            if (botCommandsByScope.Length != currentBotCommandsByScope.Length ||
                botCommandsByScope.Zip(currentBotCommandsByScope).Any(c => c.First.Command != c.Second.Command || c.First.Description != c.Second.Description))
                await client.SetMyCommandsAsync(botCommandsByScope, commandsByScope.Key, languageCode: languageId.Value, cancellationToken: token);
        }
    }

    private async Task EnsureShortDescription(ITelegramBotClient client, LanguageId languageId, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(languageId);
        
        var shortDescription = await _messageBuilder.Build(Messages.Connector_BotShortDescription, languageId);

        var currentShortDescription = await client.GetMyShortDescriptionAsync(languageId.Value, token);
        
        if (shortDescription != currentShortDescription.ShortDescription)
            await client.SetMyShortDescriptionAsync(shortDescription, languageId.Value, token);
    }
    
    private async Task EnsureDescription(ITelegramBotClient client, LanguageId languageId, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(languageId);
        
        var description = await _messageBuilder.Build(Messages.Connector_BotDescription, languageId);

        var currentDescription = await client.GetMyDescriptionAsync(languageId.Value, token);
        
        if (description != currentDescription.Description)
            await client.SetMyDescriptionAsync(description, languageId.Value, token);
    }
    
    private async Task<IReadOnlyCollection<Telegram.Bot.Types.BotCommand>> Convert(
        IEnumerable<Domain.BotCommand> botCommands,
        LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(botCommands);
        ArgumentNullException.ThrowIfNull(languageId);
        
        var commands = new List<Telegram.Bot.Types.BotCommand>();

        foreach (var botCommand in botCommands.Where(c => c.HelpMessageId is not null))
            commands.Add(new Telegram.Bot.Types.BotCommand
            {
                Command = botCommand.Value,
                Description = await _messageBuilder.Build(botCommand.HelpMessageId!, languageId)
            });

        return commands;
    }
}