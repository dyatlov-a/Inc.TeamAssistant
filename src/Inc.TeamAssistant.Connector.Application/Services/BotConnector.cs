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
    private readonly IBotReader _botReader;
    private readonly ContextCommandConverter _converter;

    public BotConnector(IBotReader botReader, ContextCommandConverter converter)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
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

    public async Task<IReadOnlyCollection<BotDetails>> GetDetails(string botToken, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(botToken);

        var client = new TelegramBotClient(botToken);
        var results = new List<BotDetails>();

        foreach (var languageId in LanguageSettings.LanguageIds)
        {
            var languageCode = languageId.Value;
            var botName = await client.GetMyNameAsync(languageCode, token);
            var botShortDescription = await client.GetMyShortDescriptionAsync(languageCode, token);
            var botDescription = await client.GetMyDescriptionAsync(languageCode, token);
            
            results.Add(new BotDetails(
                languageCode,
                botName.Name,
                botShortDescription.ShortDescription,
                botDescription.Description));
        }

        return results;
    }

    public async Task Update(Guid botId, IReadOnlyCollection<BotDetails> botDetails, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(botDetails);
        
        var bot = await _botReader.Find(botId, DateTimeOffset.UtcNow, token);
        if (bot is null)
            throw new TeamAssistantUserException(Messages.Connector_BotNotFound, botId);
        
        var client = new TelegramBotClient(bot.Token);
        
        await SetDefaultAdministratorRights(client, token);

        foreach (var item in botDetails)
        {
            await SetCommands(client, bot, new LanguageId(item.LanguageId), item.LanguageId, token);
            await SetDetails(client, item.Name, item.ShortDescription, item.Description, item.LanguageId, token);
        }

        var botDetailsItem = botDetails.Count == 1
            ? botDetails.Single()
            : botDetails.Single(b => b.LanguageId == LanguageSettings.DefaultLanguageId.Value);
        
        await SetCommands(client, bot, new LanguageId(botDetailsItem.LanguageId), targetLanguageId: null, token);
        await SetDetails(
            client,
            botDetailsItem.Name,
            botDetailsItem.ShortDescription,
            botDetailsItem.Description,
            targetLanguageId: null,
            token);
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

    private async Task SetCommands(
        ITelegramBotClient client,
        Bot bot,
        LanguageId languageId,
        string? targetLanguageId,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(languageId);

        await foreach (var commandsByScope in _converter.Convert(bot.Commands, languageId).WithCancellation(token))
        {
            await client.SetMyCommandsAsync(
                commandsByScope.Commands.OrderBy(c => c.Command),
                commandsByScope.Scope,
                languageCode: targetLanguageId,
                cancellationToken: token);
        }
    }

    private async Task SetDetails(
        ITelegramBotClient client,
        string name,
        string shortDescription,
        string description,
        string? targetLanguageId,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(shortDescription);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        
        await client.SetMyNameAsync(name, targetLanguageId, token);
        await client.SetMyShortDescriptionAsync(shortDescription, targetLanguageId, token);
        await client.SetMyDescriptionAsync(description, targetLanguageId, token);
    }
}