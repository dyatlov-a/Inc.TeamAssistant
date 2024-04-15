using Inc.TeamAssistant.Connector.Application.Extensions;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Languages;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotConstructor
{
    private readonly ChatAdministratorRights _rights = new()
    {
        CanDeleteMessages = true,
        CanPinMessages = true,
        CanManageChat = true
    };
    
    private readonly IMessageBuilder _messageBuilder;

    public BotConstructor(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task TrySetup(ITelegramBotClient client, Bot bot, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(bot);
        
        if (!await NeedToSetup(client, token))
            return;
        
        foreach (var languageId in LanguageSettings.LanguageIds)
            await Setup(client, bot, languageId, token);

        await Setup(client, bot, token);
    }

    private async Task<bool> NeedToSetup(ITelegramBotClient client, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        
        var rights = await client.GetMyDefaultAdministratorRightsAsync(cancellationToken: token);

        foreach (var (first, second) in _rights.Unwrap().Zip(rights.Unwrap()))
            if (first != second)
                return true;

        return false;
    }

    private async Task Setup(ITelegramBotClient client, Bot bot, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(bot);
        
        await client.SetMyDefaultAdministratorRightsAsync(_rights, cancellationToken: token);
    }

    private async Task Setup(ITelegramBotClient client, Bot bot, LanguageId languageId, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(languageId);
        
        var shortDescription = await _messageBuilder.Build(Messages.Connector_BotShortDescription, languageId);
        var description = await _messageBuilder.Build(Messages.Connector_BotDescription, languageId);
        var botCommands = await Convert(bot.Commands, languageId);

        await client.SetMyShortDescriptionAsync(shortDescription, languageId.Value, token);
        await client.SetMyDescriptionAsync(description, languageId.Value, token);
        await client.SetMyCommandsAsync(botCommands, languageCode: languageId.Value, cancellationToken: token);
    }

    private async Task<IReadOnlyCollection<Telegram.Bot.Types.BotCommand>> Convert(
        IEnumerable<Domain.BotCommand> botCommands,
        LanguageId languageId)
    {
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