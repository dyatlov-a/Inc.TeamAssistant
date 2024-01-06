using FluentValidation;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Gateway.Extensions;
using Inc.TeamAssistant.Gateway.Services.CommandFactories;
using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Users.Extensions;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
	private readonly IServiceProvider _serviceProvider;

    public TelegramBotMessageHandler(ILogger<TelegramBotMessageHandler> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.From?.Id;
        if (!chatId.HasValue)
	        return;
        
        var userLanguageId = LanguageSettings.DefaultLanguageId;
        using var scope = _serviceProvider.CreateScope();
        var commandFactory = scope.ServiceProvider.GetRequiredService<ICommandFactory>();
        var messageBuilder = scope.ServiceProvider.GetRequiredService<IMessageBuilder>();

        try
        {
	        var commandContext = TryCreateCommandContext(update);
	        if (commandContext is null)
		        return;

	        userLanguageId = commandContext.LanguageId;
	        var command = commandFactory.TryCreate(commandContext);
            if (command is null)
                return;

            await ProcessCommand(
	            client,
	            commandContext.ChatId,
	            commandContext.UserName,
	            command,
	            scope,
	            cancellationToken);
        }
        catch (ValidationException validationException)
        {
            await TrySend(client, chatId.Value, validationException.ToMessage(), cancellationToken);
        }
		catch (AppraiserUserException appraiserException)
		{
			var message = await messageBuilder.Build(
                appraiserException.MessageId,
                userLanguageId,
                appraiserException.Values);

            await TrySend(client, chatId.Value, message, cancellationToken);
		}
		catch (ApiRequestException ex)
        {
			_logger.LogWarning(ex, "Error from telegram API");
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Unhandled exception");

            var message = await messageBuilder.Build(Messages.UnhandledError, userLanguageId);

            await TrySend(client, chatId.Value, message, cancellationToken);
        }
    }
    
    private CommandContext? TryCreateCommandContext(Update update)
    {
	    if (update is null)
		    throw new ArgumentNullException(nameof(update));

	    if (!string.IsNullOrWhiteSpace(update.Message?.Text)
	        && update.Message.From is not null
	        && !update.Message.From.IsBot)
	    {
		    return new(
			    update.Message.Text,
			    update.Message.Chat.Id,
			    update.Message.From.Id,
			    update.Message.From.FirstName,
			    update.Message.From.GetLanguageId());
	    }

	    if (!string.IsNullOrWhiteSpace(update.CallbackQuery?.Data)
	        && update.CallbackQuery.Message is not null
	        && !update.CallbackQuery.From.IsBot)
	    {
		    return new CommandContext(
			    update.CallbackQuery.Data,
			    update.CallbackQuery.Message.Chat.Id,
			    update.CallbackQuery.From.Id,
			    update.CallbackQuery.From.FirstName,
			    update.CallbackQuery.From.GetLanguageId());
	    }

	    return null;
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

	private async Task ProcessCommand(
		ITelegramBotClient client,
		long chatId,
		string userName,
        IRequest<CommandResult> command,
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

		foreach (var notification in commandResult.Notifications)
			await ProcessNotification(client, notification, userName, scope, cancellationToken);
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
					replyMarkup: ToReplyMarkup(notificationMessage),
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
					replyMarkup: ToReplyMarkup(notificationMessage),
					cancellationToken: cancellationToken);
			}
	}

	public Task OnError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Message listened with error");

        return Task.CompletedTask;
    }
}