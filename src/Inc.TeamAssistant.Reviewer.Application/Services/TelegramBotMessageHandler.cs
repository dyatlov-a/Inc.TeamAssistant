using System.Text;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.DialogContinuations.Model;
using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.ConnectToTeam;
using Inc.TeamAssistant.Reviewer.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Reviewer.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetCanStartReview;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetTaskForReviews;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetTeams;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
    private readonly IDialogContinuation<string> _dialogContinuation;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _botName;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        IDialogContinuation<string> dialogContinuation,
        IServiceProvider serviceProvider,
        string botName)
    {
        if (string.IsNullOrWhiteSpace(botName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botName));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _botName = botName;
    }

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        var context = CommandContext.TryCreateFromMessage(update, _botName)
            ?? CommandContext.TryCreateFromQuery(update, _botName);
        if (context is null)
            return;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var translateProvider = scope.ServiceProvider.GetRequiredService<ITranslateProvider>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var currentDialog = _dialogContinuation.Find(context.Person.Id);

            if (context.Text.StartsWith(CommandList.Cancel, StringComparison.InvariantCultureIgnoreCase))
            {
                await CancelDialog(client, translateProvider, context, currentDialog, cancellationToken);
                return;
            }

            var queryResult = await mediator.Send(new GetTaskForReviewsQuery(), cancellationToken);
            foreach (var taskId in queryResult.TaskIds)
            {
                if (context.Text.StartsWith($"{CommandList.MoveToInProgress}_{taskId:N}", StringComparison.InvariantCultureIgnoreCase))
                {
                    await mediator.Send(
                        new MoveToInProgressCommand(taskId, context.Person.LanguageId),
                        cancellationToken);
                    return;
                }
                if (context.Text.StartsWith($"{CommandList.Accept}_{taskId:N}", StringComparison.InvariantCultureIgnoreCase))
                {
                    await mediator.Send(new MoveToAcceptCommand(taskId, context.Person.LanguageId), cancellationToken);
                    return;
                }
                if (context.Text.StartsWith($"{CommandList.Decline}_{taskId:N}", StringComparison.InvariantCultureIgnoreCase))
                {
                    await mediator.Send(new MoveToDeclineCommand(taskId, context.Person.LanguageId), cancellationToken);
                    return;
                }
                if (context.Text.StartsWith($"{CommandList.MoveToNextRound}_{taskId:N}", StringComparison.InvariantCultureIgnoreCase))
                {
                    await mediator.Send(new MoveToNextRoundCommand(taskId, context.Person.LanguageId), cancellationToken);
                    return;
                }
            }
            
            var command = currentDialog?.ContinuationState ?? context.Text;
            switch (command)
            {
                case CommandList.CreateTeam when !context.IsPrivate() && currentDialog is null:
                    await CreateTeam(client, translateProvider, context, cancellationToken);
                    return;
                case CommandList.CreateTeam when !context.IsPrivate() && currentDialog is not null:
                    await ContinueCreateTeam(mediator, client, translateProvider, context, currentDialog, cancellationToken);
                    return;
                case CommandList.MoveToReview when currentDialog is null:
                    await StartExecutingWithSelectTeamCommand(mediator, client, translateProvider, context, CommandList.MoveToReview, cancellationToken);
                    return;
                case CommandList.MoveToReview:
                    await ContinueMoveToReview(mediator, client, translateProvider, context, currentDialog, cancellationToken);
                    return;
                case CommandList.Leave when currentDialog is not null:
                    await ContinueLeaveTeam(mediator, client, context, currentDialog, cancellationToken);
                    return;
                case CommandList.Leave:
                    await StartExecutingWithSelectTeamCommand(mediator, client, translateProvider, context, CommandList.Leave, cancellationToken);
                    return;
                case CommandList.Help:
                    await ShowHelp(client, translateProvider, context);
                    return;
            }

            if (context.Text.StartsWith(CommandList.Start, StringComparison.InvariantCultureIgnoreCase))
            {
                var token = context.Text
                    .Replace(CommandList.Start, string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    .Trim();

                var teamId = Guid.TryParse(token, out var value) ? value : (Guid?)null;
                await mediator.Send(new ConnectToTeamCommand(teamId, context.Person), cancellationToken);
            }
        }
        catch (ApiRequestException ex)
        {
            _logger.LogWarning(ex, "Error from telegram API");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");

            await TrySend(client, context.ChatId, exception.Message, cancellationToken);
        }
    }

    private async Task CancelDialog(
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        DialogState<string>? currentDialog,
        CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        
        if (currentDialog is null)
        {
            await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_CancelDialogFail, context.Person.LanguageId),
                cancellationToken: cancellationToken);
            return;
        }
        
        _dialogContinuation.TryEnd(context.Person.Id, currentDialog.ContinuationState, context.ToChatMessage());
        foreach (var currentMessage in currentDialog.ChatMessages)
            await client.DeleteMessageAsync(currentMessage.ChatId, currentMessage.MessageId, cancellationToken);
    }

    private async Task ShowHelp(ITelegramBotClient client, ITranslateProvider translateProvider, CommandContext context)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var messageBuilder = new StringBuilder();
        if (!context.IsPrivate())
            messageBuilder.AppendLine(await translateProvider.Get(
                Messages.Reviewer_CreateTeamHelp,
                context.Person.LanguageId,
                CommandList.CreateTeam));
        messageBuilder.AppendLine(await translateProvider.Get(
            Messages.Reviewer_MoveToReviewHelp,
            context.Person.LanguageId,
            CommandList.MoveToReview));
        messageBuilder.AppendLine(await translateProvider.Get(
            Messages.Reviewer_LeaveHelp,
            context.Person.LanguageId,
            CommandList.Leave));
        messageBuilder.AppendLine(await translateProvider.Get(
            Messages.Reviewer_CancelHelp,
            context.Person.LanguageId,
            CommandList.Cancel));

        await client.SendTextMessageAsync(context.ChatId, messageBuilder.ToString());
    }

    private async Task ContinueLeaveTeam(
        IMediator mediator,
        ITelegramBotClient client,
        CommandContext context,
        DialogState<string> currentDialog,
        CancellationToken cancellationToken)
    {
        if (mediator is null)
            throw new ArgumentNullException(nameof(mediator));
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        if (currentDialog is null)
            throw new ArgumentNullException(nameof(currentDialog));
        
        if (Guid.TryParse(context.Text.TrimStart('/'), out var teamId))
        {
            await mediator.Send(
                new LeaveFromTeamCommand(teamId, context.Person.Id, context.Person.FirstName, context.Person.LanguageId),
                cancellationToken);
            
            _dialogContinuation.TryEnd(context.Person.Id, CommandList.Leave, context.ToChatMessage());
            foreach (var currentMessage in currentDialog.ChatMessages)
                await client.DeleteMessageAsync(currentMessage.ChatId, currentMessage.MessageId, cancellationToken);
        }
    }

    private async Task CreateTeam(
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        
        if (!_dialogContinuation.TryBegin(context.Person.Id, CommandList.CreateTeam, out var dialogState, context.ToChatMessage()))
        {
            await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_BeginDialogFail, context.Person.LanguageId),
                cancellationToken: cancellationToken);
            return;
        }

        var buttons = new List<InlineKeyboardButton>();
        foreach (var nextReviewerType in Enum.GetValues<NextReviewerType>().Where(t => t != NextReviewerType.None))
            buttons.Add(InlineKeyboardButton.WithCallbackData(
                await translateProvider.Get(Messages.Reviewer_NextReviewerType(nextReviewerType), context.Person.LanguageId),
                $"{nextReviewerType}"));
        var message = await client.SendTextMessageAsync(
            context.ChatId,
            await translateProvider.Get(Messages.Reviewer_EnterNextReviewerType, context.Person.LanguageId),
            replyMarkup: new InlineKeyboardMarkup(buttons),
            cancellationToken: cancellationToken);
        dialogState.TryAttachMessage(new ChatMessage(message.Chat.Id, message.MessageId));
    }

    private async Task ContinueCreateTeam(
        IMediator mediator,
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        DialogState<string> currentDialog,
        CancellationToken cancellationToken)
    {
        if (mediator is null)
            throw new ArgumentNullException(nameof(mediator));
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        
        if (currentDialog.Data.Any() && Enum.TryParse<NextReviewerType>(currentDialog.Data.Last(), out _))
        {
            await mediator.Send(
                new CreateTeamCommand(
                    context.ChatId,
                    context.Text,
                    currentDialog.Data.Last(),
                    context.Person.LanguageId),
                cancellationToken);

            _dialogContinuation.TryEnd(context.Person.Id, CommandList.CreateTeam, context.ToChatMessage());
            foreach (var currentMessage in currentDialog.ChatMessages)
                await client.DeleteMessageAsync(currentMessage.ChatId, currentMessage.MessageId, cancellationToken);
        }
        else if (Enum.TryParse<NextReviewerType>(context.Text.TrimStart('/'), out var nextReviewerType))
        {
            currentDialog.AddItem(nextReviewerType.ToString());
            
            var message = await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_EnterTeamName, context.Person.LanguageId),
                cancellationToken: cancellationToken);
            currentDialog.TryAttachMessage(new ChatMessage(message.Chat.Id, message.MessageId));
        }
    }

    private async Task StartExecutingWithSelectTeamCommand(
        IMediator mediator,
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        string targetCommand,
        CancellationToken cancellationToken)
    {
        if (mediator is null)
            throw new ArgumentNullException(nameof(mediator));
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        if (string.IsNullOrWhiteSpace(targetCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetCommand));

        var queryResult = await mediator.Send(new GetTeamsQuery(context.Person.Id, context.ChatId), cancellationToken);
        if (!queryResult.Teams.Any())
        {
            await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_HasNotTeamsForPlayer, context.Person.LanguageId),
                cancellationToken: cancellationToken);
        }
        
        if (!_dialogContinuation.TryBegin(context.Person.Id, targetCommand, out var dialogState, context.ToChatMessage()))
        {
            await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_BeginDialogFail, context.Person.LanguageId),
                cancellationToken: cancellationToken);
            return;
        }

        var buttons = queryResult.Teams.Select(t => InlineKeyboardButton.WithCallbackData(t.Name, $"/{t.Id:N}"));
        var message = await client.SendTextMessageAsync(
            context.ChatId,
            await translateProvider.Get(Messages.Reviewer_SelectTeam, context.Person.LanguageId),
            replyMarkup: new InlineKeyboardMarkup(buttons),
            cancellationToken: cancellationToken);
        dialogState.TryAttachMessage(new ChatMessage(message.Chat.Id, message.MessageId));
    }

    private async Task ContinueMoveToReview(
        IMediator mediator,
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        DialogState<string> currentDialog,
        CancellationToken cancellationToken)
    {
        if (mediator is null)
            throw new ArgumentNullException(nameof(mediator));
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        if (currentDialog is null)
            throw new ArgumentNullException(nameof(currentDialog));

        if (currentDialog.Data.Any() && Guid.TryParse(currentDialog.Data.Last(), out var teamId))
        {
            await mediator.Send(
                new MoveToReviewCommand(
                    teamId,
                    context.Person.LanguageId,
                    context.Person.Id,
                    context.Person.FirstName,
                    context.Text,
                    context.TargetUser),
                cancellationToken);

            _dialogContinuation.TryEnd(context.Person.Id, CommandList.MoveToReview, context.ToChatMessage());
            foreach (var currentMessage in currentDialog.ChatMessages)
                await client.DeleteMessageAsync(currentMessage.ChatId, currentMessage.MessageId, cancellationToken);
        }
        else if (Guid.TryParse(context.Text.TrimStart('/'), out var value))
        {
            var currentTeam = await mediator.Send(new GetCanStartReviewQuery(value), cancellationToken);
            if (!currentTeam.CanStartReview.HasValue)
            {
                await client.SendTextMessageAsync(
                    context.ChatId,
                    await translateProvider.Get(Messages.Reviewer_TeamNotFoundError, context.Person.LanguageId),
                    cancellationToken: cancellationToken);
                _dialogContinuation.TryEnd(context.Person.Id, CommandList.MoveToReview, context.ToChatMessage());
                return;
            }
            if (currentTeam.CanStartReview.Value)
            {
                var message = await client.SendTextMessageAsync(
                    context.ChatId,
                    await translateProvider.Get(Messages.Reviewer_EnterRequestForReview, context.Person.LanguageId),
                    cancellationToken: cancellationToken);
                currentDialog
                    .AddItem(value.ToString())
                    .TryAttachMessage(new ChatMessage(message.Chat.Id, message.MessageId))
                    .TryAttachMessage(context.ToChatMessage());
                return;
            }
            
            await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_TeamMinError, context.Person.LanguageId),
                cancellationToken: cancellationToken);
            _dialogContinuation.TryEnd(context.Person.Id, CommandList.MoveToReview, context.ToChatMessage());
        }
    }

    public Task OnError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Message listened with error");

        return Task.CompletedTask;
    }

    private async Task TrySend(
        ITelegramBotClient client,
        long chatId,
        string messageText,
        CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (string.IsNullOrWhiteSpace(messageText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(messageText));

        try
        {
            await client.SendTextMessageAsync(
                chatId,
                messageText,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Can not send message to chat {ChatId}", chatId);
        }
    }
}