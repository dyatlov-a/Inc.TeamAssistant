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
    private readonly DialogContinuation _dialogContinuation;

    public CommandExecutor(
        ILogger<CommandExecutor> logger,
        TelegramBotClientProvider provider,
        IServiceProvider serviceProvider,
        IMessageBuilder messageBuilder,
        IPersonRepository personRepository,
        DialogContinuation dialogContinuation)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public async Task Execute(IDialogCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        const string duplicateKeyError = "23505";

        var client = await _provider.Get(command.MessageContext.BotId, token);
        var dialog = _dialogContinuation.Find(command.MessageContext.PersonId);
        
        try
        {
            await Execute(client, command.MessageContext, command, token);
        }
        catch (ValidationException validationException)
        {
            await client.TrySend(
                dialog,
                command.MessageContext.ChatId,
                command.MessageContext.MessageId,
                validationException.ToMessage(),
                _logger,
                token);
        }
        catch (TeamAssistantUserException userException)
        {
            var errorMessage = await _messageBuilder.Build(
                userException.MessageId,
                command.MessageContext.LanguageId,
                userException.Values);
            
            await client.TrySend(
                dialog,
                command.MessageContext.ChatId,
                command.MessageContext.MessageId,
                errorMessage,
                _logger,
                token);
        }
        catch (TeamAssistantException teamAssistantException)
        {
            await client.TrySend(
                dialog,
                command.MessageContext.ChatId,
                command.MessageContext.MessageId,
                teamAssistantException.Message,
                _logger,
                token);
        }
        catch (ApiRequestException apiRequestException)
        {
            _logger.LogError(apiRequestException, "Unhandled telegram api exception");
        }
        catch (PostgresException ex) when (ex.SqlState == duplicateKeyError)
        {
            await client.TrySend(
                dialog,
                command.MessageContext.ChatId,
                command.MessageContext.MessageId,
                "Duplicate key value violates unique constraint.",
                _logger,
                token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            
            await client.TrySend(
                dialog,
                command.MessageContext.ChatId,
                command.MessageContext.MessageId,
                "An unhandled exception has occurred. Try running the command again.",
                _logger,
                token);
        }
    }
    
    private async Task Execute(
        ITelegramBotClient client,
        MessageContext messageContext,
        IRequest<CommandResult> command,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(command);

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
            ? [new MessageEntity
                {
                    Type = MessageEntityType.TextMention,
                    Offset = text.LastIndexOf(person.Name, StringComparison.InvariantCultureIgnoreCase),
                    Length = person.Name.Length,
                    User = new User
                    {
                        Id = person.Id,
                        LanguageCode = person.GetLanguageId().Value,
                        FirstName = person.Name,
                        Username = person.Username
                    }
                }]
            : Array.Empty<MessageEntity>();
    }
    
    private async Task ProcessNotification(
        ITelegramBotClient client,
        NotificationMessage notificationMessage,
        MessageContext messageContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(notificationMessage);
        ArgumentNullException.ThrowIfNull(messageContext);

        var entities = notificationMessage.TargetPersonId.HasValue
            ? await BuildMessageEntities(notificationMessage.Text, notificationMessage.TargetPersonId.Value, token)
            : Array.Empty<MessageEntity>();

        if (notificationMessage.TargetChatId.HasValue)
        {
            var message = notificationMessage.Options.Any()
                ? await client.SendPollAsync(
                    notificationMessage.TargetChatId.Value,
                    notificationMessage.Text,
                    notificationMessage.Options,
                    isAnonymous: false,
                    cancellationToken: token)
                : await client.SendTextMessageAsync(
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
                var parameter = message.Poll?.Id ?? message.MessageId.ToString();
                var command = notificationMessage.Handler(messageContext, parameter);

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
}