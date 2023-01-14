using System.Text;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.DialogContinuations;
using Inc.TeamAssistant.Reviewer.All.DialogContinuations.Model;
using Inc.TeamAssistant.Reviewer.All.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Inc.TeamAssistant.Reviewer.All.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
    private readonly ITeamRepository _teamRepository;
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IDialogContinuation _dialogContinuation;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _botLink;
    private readonly string _linkForConnectTemplate;
    private readonly string _botName;
    private readonly TimeSpan _notificationInterval;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        ITeamRepository teamRepository,
        ITaskForReviewRepository taskForReviewRepository,
        IDialogContinuation dialogContinuation,
        IServiceProvider serviceProvider,
        string botLink,
        string linkForConnectTemplate,
        string botName,
        TimeSpan notificationInterval)
    {
        if (string.IsNullOrWhiteSpace(botLink))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botLink));
        if (string.IsNullOrWhiteSpace(linkForConnectTemplate))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkForConnectTemplate));
        if (string.IsNullOrWhiteSpace(botName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botName));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _botLink = botLink;
        _linkForConnectTemplate = linkForConnectTemplate;
        _botName = botName;
        _notificationInterval = notificationInterval;
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
            var currentDialog = _dialogContinuation.Find(context.UserId);

            if (context.Text.StartsWith(CommandList.Cancel, StringComparison.InvariantCultureIgnoreCase))
            {
                await CancelDialog(client, translateProvider, context, currentDialog, cancellationToken);
                return;
            }
            
            foreach (var taskId in await _taskForReviewRepository.Get(TaskForReviewStateRules.ActiveStates, cancellationToken))
            {
                if (context.Text.StartsWith($"{CommandList.MoveToInProgress}_{taskId:N}", StringComparison.InvariantCultureIgnoreCase))
                {
                    await MoveToInProgress(client, translateProvider, context, taskId, cancellationToken);
                    return;
                }
                if (context.Text.StartsWith($"{CommandList.Accept}_{taskId:N}", StringComparison.InvariantCultureIgnoreCase))
                {
                    await MoveToAccept(client, translateProvider, context, taskId, cancellationToken);
                    return;
                }
                if (context.Text.StartsWith($"{CommandList.Decline}_{taskId:N}", StringComparison.InvariantCultureIgnoreCase))
                {
                    await MoveToDecline(client, translateProvider, context, taskId, cancellationToken);
                    return;
                }
                if (context.Text.StartsWith($"{CommandList.MoveToNextRound}_{taskId:N}", StringComparison.InvariantCultureIgnoreCase))
                {
                    await MoveToNextRound(client, translateProvider, context, taskId, cancellationToken);
                    return;
                }
            }
            
            var command = currentDialog?.ContinuationState ?? context.Text;
            switch (command)
            {
                case CommandList.CreateTeam when currentDialog is null:
                    await CreateTeam(client, translateProvider, context, cancellationToken);
                    return;
                case CommandList.CreateTeam:
                    await ContinueCreateTeam(client, translateProvider, context, currentDialog, cancellationToken);
                    return;
                case CommandList.MoveToReview when currentDialog is null:
                    await MoveToReview(client, translateProvider, context, cancellationToken);
                    return;
                case CommandList.MoveToReview:
                    await ContinueMoveToReview(client, translateProvider, context, currentDialog, cancellationToken);
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
                
                if (Guid.TryParse(token, out var teamId))
                    await ConnectToTeam(client, translateProvider, context, teamId, cancellationToken);
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
        DialogState? currentDialog,
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
                await translateProvider.Get(Messages.Reviewer_CancelDialogFail, context.LanguageId),
                cancellationToken: cancellationToken);
            return;
        }
        
        _dialogContinuation.End(context.UserId, currentDialog.ContinuationState, context.ToChatMessage());
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
        messageBuilder.AppendLine(await translateProvider.Get(
            Messages.Reviewer_CreateTeamHelp,
            context.LanguageId,
            CommandList.CreateTeam));
        messageBuilder.AppendLine(await translateProvider.Get(
            Messages.Reviewer_MoveToReviewHelp,
            context.LanguageId,
            CommandList.MoveToReview));
        messageBuilder.AppendLine(await translateProvider.Get(
            Messages.Reviewer_CancelHelp,
            context.LanguageId,
            CommandList.Cancel));

        await client.SendTextMessageAsync(context.ChatId, messageBuilder.ToString());
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

        var dialogState = _dialogContinuation.TryBegin(context.UserId, CommandList.CreateTeam, context.ToChatMessage());
        if (dialogState is null)
        {
            await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_BeginDialogFail, context.LanguageId),
                cancellationToken: cancellationToken);
            return;
        }
        
        var message = await client.SendTextMessageAsync(
            context.ChatId,
            await translateProvider.Get(Messages.Reviewer_EnterTeamName, context.LanguageId),
            cancellationToken: cancellationToken);
        dialogState.TryAttachMessage(new ChatMessage(message.Chat.Id, message.MessageId));
    }

    private async Task ContinueCreateTeam(
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        DialogState currentDialog,
        CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var newTeam = new Team(context.ChatId, context.Text);
        await _teamRepository.Upsert(newTeam, cancellationToken);

        var link = string.Format(_linkForConnectTemplate, _botLink, newTeam.Id.ToString("N"));
        var message = await client.SendTextMessageAsync(
            context.ChatId,
            await translateProvider.Get(Messages.Reviewer_ConnectToTeam, context.LanguageId, newTeam.Name, link),
            cancellationToken: cancellationToken);
        
        if (context.UserId != context.ChatId)
            await client.PinChatMessageAsync(context.ChatId, message.MessageId, cancellationToken: cancellationToken);

        _dialogContinuation.End(context.UserId, CommandList.CreateTeam, context.ToChatMessage());
        foreach (var currentMessage in currentDialog.ChatMessages)
            await client.DeleteMessageAsync(currentMessage.ChatId, currentMessage.MessageId, cancellationToken);
    }

    private async Task MoveToReview(
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

        var teams = await _teamRepository.GetTeams(context.UserId, cancellationToken);
        if (!teams.Any())
        {
            await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_HasNotTeamsForPlayer, context.LanguageId),
                cancellationToken: cancellationToken);
        }
        
        var dialogState = _dialogContinuation.TryBegin(context.UserId, CommandList.MoveToReview, context.ToChatMessage());
        if (dialogState is null)
        {
            await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_BeginDialogFail, context.LanguageId),
                cancellationToken: cancellationToken);
            return;
        }

        var buttons = teams.Select(t => InlineKeyboardButton.WithCallbackData(t.Name, $"/{t.Id:N}"));
        var message = await client.SendTextMessageAsync(
            context.ChatId,
            await translateProvider.Get(Messages.Reviewer_SelectTeam, context.LanguageId),
            replyMarkup: new InlineKeyboardMarkup(buttons),
            cancellationToken: cancellationToken);
        dialogState.TryAttachMessage(new ChatMessage(message.Chat.Id, message.MessageId));
    }

    private async Task<string> NewTaskForReviewBuild(
        ITranslateProvider translateProvider,
        CommandContext context,
        TaskForReview taskForReview)
    {
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        if (taskForReview is null)
            throw new ArgumentNullException(nameof(taskForReview));
        
        var messageBuilder = new StringBuilder();
        var messageText = await translateProvider.Get(
            Messages.Reviewer_NewTaskForReview,
            context.LanguageId,
            taskForReview.Description,
            taskForReview.Owner.Name,
            taskForReview.Reviewer.GetLogin());
        messageBuilder.AppendLine(messageText);
        messageBuilder.AppendLine();
        var state = taskForReview.State switch
        {
            TaskForReviewState.New => "⏳",
            TaskForReviewState.InProgress => "🤩",
            TaskForReviewState.OnCorrection => "😱",
            TaskForReviewState.IsArchived => "👍",
            _ => throw new ArgumentOutOfRangeException($"State {taskForReview.State} out of range for {nameof(TaskForReviewState)}.")
        };
        messageBuilder.AppendLine(state);

        return messageBuilder.ToString();
    }

    private async Task ContinueMoveToReview(
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        DialogState currentDialog,
        CancellationToken cancellationToken)
    {
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
            var currentTeam = await _teamRepository.Find(teamId, cancellationToken);
            if (currentTeam is null)
                throw new ApplicationException($"Team {teamId} was not found.");
            
            var taskForReview = currentTeam.CreateTaskForReview(context.UserId, context.Text);
            
            var message = await client.SendTextMessageAsync(
                context.ChatId,
                await NewTaskForReviewBuild(translateProvider, context, taskForReview),
                cancellationToken: cancellationToken);
            taskForReview.AttachMessage(message.MessageId);
            await _taskForReviewRepository.Upsert(taskForReview, cancellationToken);

            _dialogContinuation.End(context.UserId, CommandList.MoveToReview, context.ToChatMessage());
            foreach (var currentMessage in currentDialog.ChatMessages)
                await client.DeleteMessageAsync(currentMessage.ChatId, currentMessage.MessageId, cancellationToken);
        }
        else if (Guid.TryParse(context.Text.TrimStart('/'), out var value))
        {
            var currentTeam = await _teamRepository.Find(value, cancellationToken);
            if (currentTeam is null)
            {
                await client.SendTextMessageAsync(
                    context.ChatId,
                    await translateProvider.Get(Messages.Reviewer_TeamNotFoundError, context.LanguageId),
                    cancellationToken: cancellationToken);
                _dialogContinuation.End(context.UserId, CommandList.MoveToReview, context.ToChatMessage());
                return;
            }
            if (currentTeam.CanStartReview())
            {
                var message = await client.SendTextMessageAsync(
                    context.ChatId,
                    await translateProvider.Get(Messages.Reviewer_EnterRequestForReview, context.LanguageId),
                    cancellationToken: cancellationToken);
                currentDialog
                    .AddItem(value.ToString())
                    .TryAttachMessage(new ChatMessage(message.Chat.Id, message.MessageId))
                    .TryAttachMessage(context.ToChatMessage());
                return;
            }
            
            await client.SendTextMessageAsync(
                context.ChatId,
                await translateProvider.Get(Messages.Reviewer_TeamMinError, context.LanguageId),
                cancellationToken: cancellationToken);
            _dialogContinuation.End(context.UserId, CommandList.MoveToReview, context.ToChatMessage());
        }
    }

    private async Task ConnectToTeam(
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        Guid teamId,
        CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var team = await _teamRepository.Find(teamId, cancellationToken);
        if (team is { })
        {
            if (team.Players.All(p => p.UserId != context.UserId))
            {
                team.AddPlayer(context.LanguageId, context.UserId, context.UserName, context.UserLogin);
                await _teamRepository.Upsert(team, cancellationToken);
            }
            
            await client.SendTextMessageAsync(
                context.UserId,
                await translateProvider.Get(Messages.Reviewer_JoinToTeamSuccess, context.LanguageId, team.Name),
                cancellationToken: cancellationToken);
        }
        else
            await client.SendTextMessageAsync(
                context.UserId,
                await translateProvider.Get(Messages.Reviewer_TeamNotFoundError, context.LanguageId),
                cancellationToken: cancellationToken);
    }

    private async Task MoveToInProgress(
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        
        var taskForReview = await _taskForReviewRepository.GetById(taskId, cancellationToken);
        if (taskForReview.CanMoveToInProgress())
        {
            taskForReview.MoveToInProgress(_notificationInterval);
        
            if (taskForReview.MessageId.HasValue)
                await client.EditMessageTextAsync(
                    taskForReview.ChatId,
                    taskForReview.MessageId.Value,
                    await NewTaskForReviewBuild(translateProvider, context, taskForReview),
                    cancellationToken: cancellationToken);
            await client.SendTextMessageAsync(
                taskForReview.Reviewer.UserId,
                await translateProvider.Get(Messages.Reviewer_OperationApplied, taskForReview.Reviewer.LanguageId, cancellationToken),
                cancellationToken: cancellationToken);

            await _taskForReviewRepository.Upsert(taskForReview, cancellationToken);
        }
    }

    private async Task MoveToAccept(
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        
        var taskForReview = await _taskForReviewRepository.GetById(taskId, cancellationToken);
        if (taskForReview.CanAccept())
        {
            taskForReview.Accept();
        
            if (taskForReview.MessageId.HasValue)
                await client.EditMessageTextAsync(
                    taskForReview.ChatId,
                    taskForReview.MessageId.Value,
                    await NewTaskForReviewBuild(translateProvider, context, taskForReview),
                    cancellationToken: cancellationToken);
            await client.SendTextMessageAsync(
                taskForReview.Owner.UserId,
                await translateProvider.Get(Messages.Reviewer_Accepted, taskForReview.Owner.LanguageId, taskForReview.Description),
                cancellationToken: cancellationToken);
            await client.SendTextMessageAsync(
                taskForReview.Reviewer.UserId,
                await translateProvider.Get(Messages.Reviewer_OperationApplied, taskForReview.Reviewer.LanguageId, cancellationToken),
                cancellationToken: cancellationToken);

            await _taskForReviewRepository.Upsert(taskForReview, cancellationToken);
        }
    }

    private async Task MoveToDecline(
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var taskForReview = await _taskForReviewRepository.GetById(taskId, cancellationToken);
        if (taskForReview.CanDecline())
        {
            taskForReview.Decline();

            if (taskForReview.MessageId.HasValue)
                await client.EditMessageTextAsync(
                    taskForReview.ChatId,
                    taskForReview.MessageId.Value,
                    await NewTaskForReviewBuild(translateProvider, context, taskForReview),
                    cancellationToken: cancellationToken);
            await client.SendTextMessageAsync(
                taskForReview.Reviewer.UserId,
                await translateProvider.Get(Messages.Reviewer_OperationApplied, taskForReview.Reviewer.LanguageId, cancellationToken),
                cancellationToken: cancellationToken);

            await _taskForReviewRepository.Upsert(taskForReview, cancellationToken);
        }
    }

    private async Task MoveToNextRound(
        ITelegramBotClient client,
        ITranslateProvider translateProvider,
        CommandContext context,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        
        var taskForReview = await _taskForReviewRepository.GetById(taskId, cancellationToken);
        if (taskForReview.CanMoveToNextRound())
        {
            taskForReview.MoveToNextRound();
        
            if (taskForReview.MessageId.HasValue)
                await client.EditMessageTextAsync(
                    taskForReview.ChatId,
                    taskForReview.MessageId.Value,
                    await NewTaskForReviewBuild(translateProvider, context, taskForReview),
                    cancellationToken: cancellationToken);
            await client.SendTextMessageAsync(
                taskForReview.Owner.UserId,
                await translateProvider.Get(Messages.Reviewer_OperationApplied, taskForReview.Owner.LanguageId, cancellationToken),
                cancellationToken: cancellationToken);

            await _taskForReviewRepository.Upsert(taskForReview, cancellationToken);
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