using FluentValidation;
using Inc.TeamAssistant.Connector.Application.Extensions;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
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
    private readonly ITeamAccessor _teamAccessor;
    private readonly DialogContinuation _dialogContinuation;

    public CommandExecutor(
        ILogger<CommandExecutor> logger,
        TelegramBotClientProvider provider,
        IServiceProvider serviceProvider,
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        DialogContinuation dialogContinuation)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public async Task Execute(IDialogCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        const string duplicateKeyError = "23505";
        
        var client = await _provider.Get(command.MessageContext.Bot.Id, token);
        var dialog = _dialogContinuation.Find(command.MessageContext.Bot.Id, command.MessageContext.TargetChat);
        
        try
        {
            await Execute(client, command.MessageContext, command, token);
        }
        catch (ValidationException validationException)
        {
            await client.TrySend(
                dialog,
                command.MessageContext.ChatMessage,
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
                command.MessageContext.ChatMessage,
                errorMessage,
                _logger,
                token);
        }
        catch (TeamAssistantException teamAssistantException)
        {
            await client.TrySend(
                dialog,
                command.MessageContext.ChatMessage,
                teamAssistantException.Message,
                _logger,
                token);
        }
        catch (ApiRequestException apiRequestException)
        {
            _logger.LogWarning(apiRequestException, "Unhandled telegram api exception");
        }
        catch (PostgresException ex) when (ex.SqlState == duplicateKeyError)
        {
            await client.TrySend(
                dialog,
                command.MessageContext.ChatMessage,
                "Duplicate key value violates unique constraint.",
                _logger,
                token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on execute command");
            
            await client.TrySend(
                dialog,
                command.MessageContext.ChatMessage,
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
        Guid botId,
        NotificationMessage notificationMessage,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(notificationMessage);
        
        var results = new List<MessageEntity>();

        foreach (var target in notificationMessage.TargetPersons)
        {
            var languageId = await _teamAccessor.GetClientLanguage(botId, target.Person.Id, token);
            
            results.Add(new MessageEntity
            {
                Type = MessageEntityType.TextMention,
                Offset = target.Offset,
                Length = target.Person.Name.Length,
                User = new User
                {
                    Id = target.Person.Id,
                    LanguageCode = languageId.Value,
                    FirstName = target.Person.Name,
                    Username = target.Person.Username
                }
            });
        }
        
        return results;
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

        var entities = notificationMessage.TargetPersons.Any()
            ? await BuildMessageEntities(messageContext.Bot.Id, notificationMessage, token)
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
                    replyToMessageId: notificationMessage.ReplyToMessageId,
                    cancellationToken: token);

            if (notificationMessage.Pinned)
                await client.TryPinChatMessage(
                    new(notificationMessage.TargetChatId.Value, message.MessageId),
                    _logger,
                    token);

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
            await client.TryDeleteMessage(notificationMessage.DeleteMessage, _logger, token);
    }
}