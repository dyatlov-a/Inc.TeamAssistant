using System.Text;
using FluentValidation;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
    private readonly IBotRepository _botRepository;
    private readonly IDialogContinuation<BotCommandStage> _dialogContinuation;
    private readonly IPersonRepository _personRepository;
    private readonly CommandFactory _commandFactory;
    private readonly IServiceProvider _serviceProvider;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        IBotRepository botRepository,
        IDialogContinuation<BotCommandStage> dialogContinuation,
        IPersonRepository personRepository,
        CommandFactory commandFactory,
        IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task Handle(ITelegramBotClient client, Update update, Guid botId, CancellationToken token)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        var bot = await _botRepository.Find(botId, token);
        if (bot is null)
            throw new ApplicationException($"Bot {botId} was not found.");

        var messageContext = await CreateMessageContext(update, bot, token);
        if (messageContext is null)
            return;
        
        try
        {
            foreach (var botCommand in bot.Commands)
                if (messageContext.Cmd?.StartsWith(
                        botCommand.Value,
                        StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    var command = await _commandFactory.TryCreate(messageContext);

                    if (command is not null)
                    {
                        await Execute(client, messageContext, command, token);
                        return;
                    }
                }
        }
        catch (ValidationException validationException)
        {
            await TrySend(client, messageContext.ChatId, ToMessage(validationException), token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await TrySend(
                client,
                messageContext.ChatId,
                "An unhandled exception has occurred. Try running the command again.",
                token);
        }
    }

    public Task OnError(ITelegramBotClient client, Exception exception, string botName, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(botName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botName));

        _logger.LogError(exception, "Bot {BotName} listened message with error", botName);

        return Task.CompletedTask;
    }

    private async Task<MessageContext?> CreateMessageContext(Update update, Bot bot, CancellationToken token)
    {
        if (!string.IsNullOrWhiteSpace(update.Message?.Text)
            && update.Message.From is not null
            && !update.Message.From.IsBot)
        {
            var person = await EnsurePerson(update.Message.From, token);
            var inProgressCommand = _dialogContinuation.Find(update.Message.From.Id);
            var targetTeams = bot.Teams
                .Where(t => t.Teammates.Any(tm => tm.Id == person.Id))
                .Select(p => (p.Id, p.Name))
                .ToArray();
            
            return new(
                bot.Id,
                bot.Name,
                targetTeams,
                inProgressCommand is not null
                    ? inProgressCommand.Data.First()
                    : update.Message.Text,
                update.Message.Text,
                update.Message.Chat.Id,
                update.Message.From.Id,
                update.Message.From.FirstName,
                update.Message.From.Username,
                update.Message.MessageId,
                person.LanguageId,
                inProgressCommand?.ContinuationState);
        }

        if (!string.IsNullOrWhiteSpace(update.CallbackQuery?.Data)
            && update.CallbackQuery.Message is not null
            && !update.CallbackQuery.From.IsBot)
        {
            var person = await EnsurePerson(update.CallbackQuery.From, token);
            var inProgressCommand = _dialogContinuation.Find(update.CallbackQuery.From.Id);
            var targetTeams = bot.Teams
                .Where(t => t.Teammates.Any(tm => tm.Id == person.Id))
                .Select(p => (p.Id, p.Name))
                .ToArray();
            
            return new(
                bot.Id,
                bot.Name,
                targetTeams,
                inProgressCommand is not null ? inProgressCommand.Data.First() : update.CallbackQuery.Data,
                update.CallbackQuery.Data,
                update.CallbackQuery.Message.Chat.Id,
                update.CallbackQuery.From.Id,
                update.CallbackQuery.From.FirstName,
                update.CallbackQuery.From.Username,
                update.CallbackQuery.Message.MessageId,
                person.LanguageId,
                inProgressCommand?.ContinuationState);
        }

        return null;
    }

    private async Task<Person> EnsurePerson(User user, CancellationToken token)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        var languageId = string.IsNullOrWhiteSpace(user.LanguageCode)
            ? LanguageSettings.DefaultLanguageId
            : new LanguageId(user.LanguageCode);
        var person = new Person(
            user.Id,
            user.FirstName,
            languageId,
            user.Username);
        
        await _personRepository.Upsert(person, token);

        return person;
    }

    private async Task Execute(
        ITelegramBotClient client,
        MessageContext messageContext,
        IRequest<CommandResult> command,
        CancellationToken token)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        using var scope = _serviceProvider.CreateScope();
        var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
        var commandResult = await mediatr.Send(command, token);

        foreach (var notification in commandResult.Notifications)
            await ProcessNotification(client, notification, messageContext, token);
    }
    
    private async Task ProcessNotification(
        ITelegramBotClient client,
        NotificationMessage notificationMessage,
        MessageContext messageContext,
        CancellationToken token)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (notificationMessage is null)
            throw new ArgumentNullException(nameof(notificationMessage));
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        if (notificationMessage.TargetChatIds?.Any() == true)
            foreach (var targetChatId in notificationMessage.TargetChatIds)
            {
                var message = await client.SendTextMessageAsync(
                    targetChatId,
                    notificationMessage.Text,
                    replyMarkup: ToReplyMarkup(notificationMessage),
                    cancellationToken: token);

                if (notificationMessage.Pinned)
                    await client.PinChatMessageAsync(
                        new ChatId(targetChatId),
                        message.MessageId,
                        cancellationToken: token);

                if (notificationMessage.Handler is not null)
                {
                    var command = notificationMessage.Handler(
                        targetChatId,
                        messageContext.FirstName,
                        message.MessageId);

                    await Execute(client, messageContext, command, token);
                }
            }

        if (notificationMessage.TargetMessages?.Any() == true)
            foreach (var message in notificationMessage.TargetMessages)
                await client.EditMessageTextAsync(
                    new(message.ChatId),
                    message.MessageId,
                    notificationMessage.Text,
                    replyMarkup: ToReplyMarkup(notificationMessage),
                    cancellationToken: token);
        
        if (notificationMessage.DeleteMessages?.Any() == true)
            foreach (var message in notificationMessage.DeleteMessages)
                await client.DeleteMessageAsync(new(message.ChatId), message.MessageId, token);
    }
    
    private InlineKeyboardMarkup? ToReplyMarkup(NotificationMessage message)
    {
        const int rowCapacity = 7;
		
        return message.Buttons.Any()
            ? new InlineKeyboardMarkup(message.Buttons
                .Select(b => InlineKeyboardButton.WithCallbackData(b.Text, b.Data))
                .Chunk(rowCapacity))
            : null;
    }
    
    private async Task TrySend(ITelegramBotClient client, long chatId, string message, CancellationToken token)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));

        try
        {
            await client.SendTextMessageAsync(
                chatId,
                message,
                cancellationToken: token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Can not send message to chat {TargetChatId}", chatId);
        }
    }
    
    private static string ToMessage(ValidationException validationException)
    {
        if (validationException is null)
            throw new ArgumentNullException(nameof(validationException));

        return validationException.Errors.Any()
            ? validationException.Errors.Aggregate(
                new StringBuilder(),
                (sb, e) =>
                {
                    sb.AppendLine(e.ErrorMessage);
                    return sb;
                },
                sb => sb.ToString())
            : validationException.Message;
    }
}