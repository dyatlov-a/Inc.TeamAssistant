using FluentValidation;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Extensions;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandExecutor : ICommandExecutor
{
    private readonly ILogger<CommandExecutor> _logger;
    private readonly TelegramBotClientProvider _provider;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBuilder _messageBuilder;
    private readonly IPersonRepository _personRepository;

    public CommandExecutor(
        ILogger<CommandExecutor> logger,
        TelegramBotClientProvider provider,
        IServiceProvider serviceProvider,
        IMessageBuilder messageBuilder,
        IPersonRepository personRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task Execute(IDialogCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        const string duplicateKeyError = "23505";

        var client = await _provider.Get(command.MessageContext.BotId, token);
        
        try
        {
            await Execute(client, command.MessageContext, command, token);
        }
        catch (ValidationException validationException)
        {
            await TrySend(client, command.MessageContext.ChatId, validationException.ToMessage(), token);
        }
        catch (TeamAssistantUserException userException)
        {
            var errorMessage = await _messageBuilder.Build(
                userException.MessageId,
                command.MessageContext.LanguageId,
                userException.Values);

            await TrySend(client, command.MessageContext.ChatId, errorMessage, token);
        }
        catch (TeamAssistantException teamAssistantException)
        {
            await TrySend(
                client,
                command.MessageContext.ChatId,
                teamAssistantException.Message,
                token);
        }
        catch (ApiRequestException apiRequestException)
        {
            _logger.LogError(apiRequestException, "Unhandled telegram api exception");
        }
        catch (PostgresException ex) when (ex.SqlState == duplicateKeyError)
        {
            await TrySend(
                client,
                command.MessageContext.ChatId,
                "Duplicate key value violates unique constraint.",
                token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await TrySend(
                client,
                command.MessageContext.ChatId,
                "An unhandled exception has occurred. Try running the command again.",
                token);
        }
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

    private async Task<IReadOnlyCollection<MessageEntity>> BuildMessageEntities(
        string text,
        long personId,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));
        
        var person = await _personRepository.Find(personId, token);

        return person is not null
            ? new[]
            {
                new MessageEntity
                {
                    Type = MessageEntityType.TextMention,
                    Offset = text.LastIndexOf(person.Name, StringComparison.InvariantCultureIgnoreCase),
                    Length = person.Name.Length,
                    User = new User
                    {
                        Id = person.Id,
                        LanguageCode = person.LanguageId.Value,
                        FirstName = person.Name,
                        Username = person.Username
                    }
                }
            }
            : Array.Empty<MessageEntity>();
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

        var entities = notificationMessage.TargetPersonId.HasValue
            ? await BuildMessageEntities(notificationMessage.Text, notificationMessage.TargetPersonId.Value, token)
            : Array.Empty<MessageEntity>();

        if (notificationMessage.TargetChatId.HasValue)
        {
            var message = await client.SendTextMessageAsync(
                notificationMessage.TargetChatId.Value,
                notificationMessage.Text,
                replyMarkup: notificationMessage.ToReplyMarkup(),
                entities: entities,
                cancellationToken: token);

            if (notificationMessage.Pinned)
                await client.PinChatMessageAsync(
                    new ChatId(notificationMessage.TargetChatId.Value),
                    message.MessageId,
                    cancellationToken: token);

            if (notificationMessage.Handler is not null)
            {
                var command = notificationMessage.Handler(messageContext, message.MessageId);

                await Execute(client, messageContext, command, token);
            }
        }

        if (notificationMessage.TargetMessage is not null)
            await client.EditMessageTextAsync(
                new(notificationMessage.TargetMessage.ChatId),
                notificationMessage.TargetMessage.MessageId,
                notificationMessage.Text,
                replyMarkup: notificationMessage.ToReplyMarkup(),
                entities: entities,
                cancellationToken: token);
        
        if (notificationMessage.DeleteMessage is not null)
            await client.DeleteMessageAsync
                (new(notificationMessage.DeleteMessage.ChatId),
                    notificationMessage.DeleteMessage.MessageId,
                    token);
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
}