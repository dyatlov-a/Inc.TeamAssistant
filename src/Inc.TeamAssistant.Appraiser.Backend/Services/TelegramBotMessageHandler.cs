using System.Runtime.CompilerServices;
using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Backend.Services.CommandFactories;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using MediatR;
using Inc.TeamAssistant.Appraiser.Backend.Extensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
	private readonly IServiceProvider _serviceProvider;
    private readonly IUserSettingsProvider _userSettingsProvider;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        IServiceProvider serviceProvider,
        IUserSettingsProvider userSettingsProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _userSettingsProvider = userSettingsProvider ?? throw new ArgumentNullException(nameof(userSettingsProvider));
    }

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        if (update.Message?.From is null || update.Message.From.IsBot)
            return;

        using var scope = _serviceProvider.CreateScope();
        var commandFactory = scope.ServiceProvider.GetRequiredService<ICommandFactory>();
        var messageBuilder = scope.ServiceProvider.GetRequiredService<IMessageBuilder>();

        var userName = await _userSettingsProvider.GetUserName(new(update.Message.From!.Id), cancellationToken);
        var userLanguageId = update.Message.From.GetLanguageId();

        try
        {
            var command = !string.IsNullOrWhiteSpace(update.Message!.Text)
                ? commandFactory.TryCreate(new(
                    update.Message.Text,
                    update.Message.Chat.Id,
                    new(update.Message.From!.Id),
                    userName,
                    update.Message.From.GetUserName(),
                    userLanguageId))
                : null;
            if (command is null)
                return;

            await ProcessCommand(client, update.Message.Chat.Id, userName, command, scope, cancellationToken);
        }
        catch (ValidationException validationException)
        {
            await TrySend(client, update.Message.Chat.Id, validationException.ToMessage(), cancellationToken);
        }
		catch (AppraiserUserException appraiserException)
		{
			var message = await messageBuilder.Build(
                appraiserException.MessageId,
                userLanguageId,
                appraiserException.Values);

            await TrySend(client, update.Message.Chat.Id, message, cancellationToken);
		}
		catch (ApiRequestException ex)
        {
			_logger.LogWarning(ex, "Error from telegram API");
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Unhandled exception");

            var message = await messageBuilder.Build(Messages.UnhandledError, userLanguageId);

            await TrySend(client, update.Message.Chat.Id, message, cancellationToken);
        }
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
            _logger.LogError(ex, "Can not send message to chat {ChatId}", chatId);
        }
    }

	private async Task ProcessCommand(
		ITelegramBotClient client,
		long chatId,
		string userName,
        IBaseRequest command,
		IServiceScope scope,
        CancellationToken cancellationToken)
	{
		if (client is null)
			throw new ArgumentNullException(nameof(client));
		if (string.IsNullOrWhiteSpace(userName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
        if (command is null)
			throw new ArgumentNullException(nameof(command));
		if (scope is null)
			throw new ArgumentNullException(nameof(scope));

		var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

		var commandResult = await mediator.Send(command, cancellationToken);
		if (commandResult is null)
			return;

		var notificationProvider = (IAsyncEnumerable<NotificationMessage>) Build(
			(dynamic) commandResult,
			chatId,
			scope,
			cancellationToken);

		await foreach (var notification in notificationProvider.WithCancellation(cancellationToken))
			await ProcessNotification(client, notification, userName, scope, cancellationToken);
	}

	private async Task ProcessNotification(
		ITelegramBotClient client,
		NotificationMessage notificationMessage,
		string userName,
        IServiceScope scope,
        CancellationToken cancellationToken)
	{
		if (client is null)
			throw new ArgumentNullException(nameof(client));
		if (notificationMessage is null)
			throw new ArgumentNullException(nameof(notificationMessage));
		if (string.IsNullOrWhiteSpace(userName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
        if (scope is null)
			throw new ArgumentNullException(nameof(scope));

        if (notificationMessage.TargetChatIds?.Any() == true)
			foreach (var targetChatId in notificationMessage.TargetChatIds)
			{
				var message = await client.SendTextMessageAsync(
					targetChatId,
                    notificationMessage.Text,
					cancellationToken: cancellationToken);

				if (notificationMessage.Handler is not null)
				{
					var command = notificationMessage.Handler(
						targetChatId,
						userName,
						message.MessageId);

					await ProcessCommand(client, targetChatId, userName, command, scope, cancellationToken);
				}
			}

		if (notificationMessage.TargetMessages?.Any() == true)
			foreach (var message in notificationMessage.TargetMessages)
			{
				await client.EditMessageTextAsync(
					new(message.ChatId),
					message.MessageId,
                    notificationMessage.Text,
					cancellationToken: cancellationToken);
			}
	}

	private async IAsyncEnumerable<NotificationMessage> Build<TCommandResult>(
		TCommandResult commandResult,
		long fromId,
		IServiceScope scope,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));
		if (scope is null)
			throw new ArgumentNullException(nameof(scope));

		var notificationBuilder = scope.ServiceProvider.GetService<INotificationBuilder<TCommandResult>>();

		if (notificationBuilder is null)
			yield break;

		await foreach (var notification in notificationBuilder
										   .Build(commandResult, fromId)
										   .WithCancellation(cancellationToken))
			yield return notification;
	}

	public Task OnError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Message listened with error");

        return Task.CompletedTask;
    }
}