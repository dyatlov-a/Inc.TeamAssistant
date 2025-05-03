using FluentValidation;
using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Connector.Application.Telegram;
using Inc.TeamAssistant.Connector.Domain;
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

namespace Inc.TeamAssistant.Connector.Application.Executors;

internal sealed class CommandExecutor : ICommandExecutor
{
    private readonly ILogger<CommandExecutor> _logger;
    private readonly TelegramBotClientProvider _provider;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBuilder _messageBuilder;
    private readonly DialogContinuation _dialogContinuation;
    private readonly TelegramMessageSender _messageSender;

    public CommandExecutor(
        ILogger<CommandExecutor> logger,
        TelegramBotClientProvider provider,
        IServiceProvider serviceProvider,
        IMessageBuilder messageBuilder,
        DialogContinuation dialogContinuation,
        TelegramMessageSender messageSender)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
    }

    public async Task Execute(IDialogCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        const string duplicateKeyError = "23505";
        
        var client = await _provider.Get(command.MessageContext.Bot.Id, token);
        var dialog = _dialogContinuation.Find(command.MessageContext.Bot.Id, command.MessageContext.TargetChat);
        
        try
        {
            await ProcessCommand(client, command.MessageContext, command, token);
        }
        catch (ValidationException validationException)
        {
            await TrySend(
                client,
                dialog,
                command.MessageContext.ChatMessage,
                ValidationMessageBuilder.Build(validationException),
                token);
        }
        catch (TeamAssistantUserException userException)
        {
            var errorMessage = _messageBuilder.Build(
                userException.MessageId,
                command.MessageContext.LanguageId,
                userException.Values);
            
            await TrySend(
                client,
                dialog,
                command.MessageContext.ChatMessage,
                errorMessage,
                token);
        }
        catch (TeamAssistantException teamAssistantException)
        {
            await TrySend(
                client,
                dialog,
                command.MessageContext.ChatMessage,
                teamAssistantException.Message,
                token);
        }
        catch (ApiRequestException apiRequestException)
        {
            _logger.LogWarning(apiRequestException, "Unhandled telegram api exception");
        }
        catch (PostgresException ex) when (ex.SqlState == duplicateKeyError)
        {
            await TrySend(
                client,
                dialog,
                command.MessageContext.ChatMessage,
                "Duplicate key value violates unique constraint.",
                token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on execute command");
            
            await TrySend(
                client,
                dialog,
                command.MessageContext.ChatMessage,
                "An unhandled exception has occurred. Try running the command again.",
                token);
        }
    }
    
    private async Task ProcessCommand(
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
        {
            var continuation = await _messageSender.Send(client, notification, messageContext, token);
            if (continuation is not null)
                await ProcessCommand(client, messageContext, continuation, token);
        }
    }
    
    private async Task TrySend(
        ITelegramBotClient client,
        DialogState? dialog,
        ChatMessage chatMessage,
        string text,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(chatMessage);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        try
        {
            var message = await client.SendMessage(
                chatId: chatMessage.ChatId,
                text: text,
                cancellationToken: token);
            
            if (dialog is not null)
            {
                dialog.Attach(chatMessage);
                dialog.Attach(chatMessage with { MessageId = message.MessageId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bot can not send message to chat {TargetChatId}", chatMessage.ChatId);
        }
    }
}