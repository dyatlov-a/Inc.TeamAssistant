using System.Text;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.DialogContinuations;
using Inc.TeamAssistant.Reviewer.All.DialogContinuations.Model;
using Inc.TeamAssistant.Reviewer.All.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Inc.TeamAssistant.Reviewer.All.Extensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

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

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        ITeamRepository teamRepository,
        ITaskForReviewRepository taskForReviewRepository,
        IDialogContinuation dialogContinuation,
        IServiceProvider serviceProvider,
        string botLink,
        string linkForConnectTemplate,
        string botName)
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
    }

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        if (update.Message?.From is null || update.Message.From.IsBot || string.IsNullOrWhiteSpace(update.Message.Text))
            return;

        using var scope = _serviceProvider.CreateScope();
        var translateProvider = scope.ServiceProvider.GetRequiredService<ITranslateProvider>();
        var languageId = update.Message.From.GetLanguageId();
        var commandText = update.Message.Text.TrimEnd($"@{_botName}".ToCharArray());

        if (update.Message.From.Id == update.Message.Chat.Id
            && !commandText.StartsWith(CommandList.Start, StringComparison.InvariantCultureIgnoreCase)
            && !commandText.StartsWith(CommandList.Accept, StringComparison.InvariantCultureIgnoreCase)
            && !commandText.StartsWith(CommandList.Decline, StringComparison.InvariantCultureIgnoreCase)
            && !commandText.StartsWith(CommandList.MoveToNextRound, StringComparison.InvariantCultureIgnoreCase))
        {
            var messageText = await translateProvider.Get(Messages.Reviewer_GetStarted, languageId);
            await client.SendTextMessageAsync(update.Message.From.Id, messageText, cancellationToken: cancellationToken);
            return;
        }

        try
        {
            var context = new CommandContext(
                client,
                translateProvider,
                update.Message.MessageId,
                update.Message.From.Id,
                update.Message.From.GetUserName(),
                update.Message.From.Username,
                languageId,
                update.Message.Chat.Id,
                commandText);
            var currentDialog = _dialogContinuation.Find(update.Message.From.Id);

            if (context.Text.StartsWith(CommandList.Cancel, StringComparison.InvariantCultureIgnoreCase))
            {
                await CancelDialog(context, currentDialog, cancellationToken);
                return;
            }
            
            var command = currentDialog?.ContinuationState ?? context.Text.Trim();
            switch (command)
            {
                case CommandList.CreateTeam when currentDialog is null:
                    await CreateTeam(context, cancellationToken);
                    return;
                case CommandList.CreateTeam:
                    await ContinueCreateTeam(context, currentDialog, cancellationToken);
                    return;
                case CommandList.MoveToReview when currentDialog is null:
                    await MoveToReview(context, cancellationToken);
                    return;
                case CommandList.MoveToReview:
                    await ContinueMoveToReview(context, currentDialog, cancellationToken);
                    return;
                case CommandList.Help:
                    await ShowHelp(context);
                    return;
            }

            if (context.Text.StartsWith(CommandList.Start, StringComparison.InvariantCultureIgnoreCase))
            {
                var token = context.Text.ToLower().TrimStart(CommandList.Start.ToCharArray()).Trim();
                if (Guid.TryParse(token, out var teamId))
                {
                    await ConnectToTeam(context, teamId, cancellationToken);
                    return;
                }
            }

            foreach (var activeTask in await _taskForReviewRepository.Get(TaskForReviewStateRules.ActiveStates, cancellationToken))
            {
                if (context.Text.StartsWith($"{CommandList.Accept}_{activeTask:N}", StringComparison.InvariantCultureIgnoreCase))
                    await MoveToState(activeTask, async (t, c) =>
                    {
                        var messageText = await translateProvider.Get(Messages.Reviewer_Accepted, t.Owner.LanguageId, t.Description);
                        await client.SendTextMessageAsync(t.Owner.UserId, messageText, cancellationToken: cancellationToken);
                        t.Accept();
                    }, cancellationToken);
                if (context.Text.StartsWith($"{CommandList.Decline}_{activeTask:N}", StringComparison.InvariantCultureIgnoreCase))
                    await MoveToState(activeTask, (t, c) =>
                    {
                        t.Decline();
                        return Task.CompletedTask;
                    }, cancellationToken);
                if (context.Text.StartsWith($"{CommandList.MoveToNextRound}_{activeTask:N}", StringComparison.InvariantCultureIgnoreCase))
                    await MoveToState(activeTask, (t, c) =>
                    {
                        t.MoveToNextRound();
                        return Task.CompletedTask;
                    }, cancellationToken);
            }
        }
        catch (ApiRequestException ex)
        {
            _logger.LogWarning(ex, "Error from telegram API");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");

            await TrySend(client, update.Message.Chat.Id, exception.Message, cancellationToken);
        }
    }

    private async Task CancelDialog(
        CommandContext context,
        DialogState? currentDialog,
        CancellationToken cancellationToken)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        
        if (currentDialog is { })
        {
            _dialogContinuation.End(context.UserId, currentDialog.ContinuationState, context.MessageId);
            foreach (var currentMessageId in currentDialog.MessageIds)
                await context.Client.DeleteMessageAsync(context.ChatId, currentMessageId, cancellationToken);
        }
        else
        {
            var messageText = await context.TranslateProvider.Get(Messages.Reviewer_CancelDialogFail, context.LanguageId);
            await context.Client.SendTextMessageAsync(context.ChatId, messageText, cancellationToken: cancellationToken);
        }
    }

    private async Task ShowHelp(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await context.TranslateProvider.Get(
            Messages.Reviewer_CreateTeamHelp,
            context.LanguageId,
            CommandList.CreateTeam));
        messageBuilder.AppendLine(await context.TranslateProvider.Get(
            Messages.Reviewer_MoveToReviewHelp,
            context.LanguageId,
            CommandList.MoveToReview));
        messageBuilder.AppendLine(await context.TranslateProvider.Get(
            Messages.Reviewer_CancelHelp,
            context.LanguageId,
            CommandList.Cancel));

        await context.Client.SendTextMessageAsync(context.ChatId, messageBuilder.ToString());
    }

    private async Task CreateTeam(CommandContext context, CancellationToken cancellationToken)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var dialogState = _dialogContinuation.TryBegin(context.UserId, CommandList.CreateTeam, context.MessageId);

        if (dialogState is { })
        {
            var message = await context.Client.SendTextMessageAsync(
                context.ChatId,
                await context.TranslateProvider.Get(Messages.Reviewer_EnterTeamName, context.LanguageId),
                cancellationToken: cancellationToken);

            dialogState.AttachMessage(message.MessageId);
        }
        else
            await context.Client.SendTextMessageAsync(
                context.ChatId,
                await context.TranslateProvider.Get(Messages.Reviewer_BeginDialogFail, context.LanguageId),
                cancellationToken: cancellationToken);
    }

    private async Task ContinueCreateTeam(
        CommandContext context,
        DialogState currentDialog,
        CancellationToken cancellationToken)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        if (currentDialog is null)
            throw new ArgumentNullException(nameof(currentDialog));

        var newTeam = new Team(context.ChatId, context.Text);
        await _teamRepository.Upsert(newTeam, cancellationToken);

        var link = string.Format(_linkForConnectTemplate, _botLink, newTeam.Id.ToString("N"));
        var message = await context.Client.SendTextMessageAsync(
            context.ChatId,
            await context.TranslateProvider.Get(Messages.Reviewer_ConnectToTeam, context.LanguageId, newTeam.Name, link),
            cancellationToken: cancellationToken);
        await context.Client.PinChatMessageAsync(
            context.ChatId,
            message.MessageId,
            cancellationToken: cancellationToken);

        _dialogContinuation.End(context.UserId, CommandList.CreateTeam, context.MessageId);
        foreach (var currentMessageId in currentDialog.MessageIds)
            await context.Client.DeleteMessageAsync(context.ChatId, currentMessageId, cancellationToken);
    }

    private async Task MoveToReview(CommandContext context, CancellationToken cancellationToken)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        
        var dialogState = _dialogContinuation.TryBegin(context.UserId, CommandList.MoveToReview, context.MessageId);

        if (dialogState is { })
        {
            var teams = (await _teamRepository.GetTeams(context.ChatId, cancellationToken)).OrderBy(t => t.Name).ToArray();
            
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(await context.TranslateProvider.Get(Messages.Reviewer_SelectTeam, context.LanguageId));
            for (var index = 0; index < teams.Length; index++)
            {
                messageBuilder.AppendLine();
                messageBuilder.AppendLine($"{index + 1}. {teams[index].Name} /{teams[index].Id.ToString("N")}");
            }

            var message = await context.Client.SendTextMessageAsync(
                context.ChatId,
                messageBuilder.ToString(),
                cancellationToken: cancellationToken);
            dialogState.AttachMessage(message.MessageId);
        }
        else
            await context.Client.SendTextMessageAsync(
                context.ChatId,
                await context.TranslateProvider.Get(Messages.Reviewer_BeginDialogFail, context.LanguageId),
                cancellationToken: cancellationToken);
    }

    private async Task ContinueMoveToReview(
        CommandContext context,
        DialogState currentDialog,
        CancellationToken cancellationToken)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        if (currentDialog is null)
            throw new ArgumentNullException(nameof(currentDialog));

        if (currentDialog.Data.Any() && Guid.TryParse(currentDialog.Data.Last().TrimStart('/'), out var teamId))
        {
            var currentTeam = await _teamRepository.Find(teamId, cancellationToken);
            var taskForReview = currentTeam!.CreateTaskForReview(context.UserId, context.Text);

            await _taskForReviewRepository.Upsert(taskForReview, cancellationToken);

            var messageText = await context.TranslateProvider.Get(
                Messages.Reviewer_NewTaskForReview,
                context.LanguageId,
                taskForReview.Description,
                taskForReview.Reviewer.GetLogin());
            await context.Client.SendTextMessageAsync(context.ChatId, messageText, cancellationToken: cancellationToken);

            _dialogContinuation.End(context.UserId, CommandList.MoveToReview, context.MessageId);
            foreach (var currentMessageId in currentDialog.MessageIds)
                await context.Client.DeleteMessageAsync(context.ChatId, currentMessageId, cancellationToken);
        }
        else if (Guid.TryParse(context.Text.TrimStart('/'), out var value))
        {
            const int minTeamCount = 2;
            var currentTeam = await _teamRepository.Find(value, cancellationToken);
            if (currentTeam is null)
            {
                await context.Client.SendTextMessageAsync(
                    context.ChatId,
                    await context.TranslateProvider.Get(Messages.Reviewer_TeamNotFoundError, context.LanguageId),
                    cancellationToken: cancellationToken);

                _dialogContinuation.End(context.UserId, CommandList.MoveToReview, context.MessageId);
                return;
            }
            if (currentTeam.Players.Count < minTeamCount)
            {
                await context.Client.SendTextMessageAsync(
                    context.ChatId,
                    await context.TranslateProvider.Get(Messages.Reviewer_TeamMinError, context.LanguageId, minTeamCount),
                    cancellationToken: cancellationToken);

                _dialogContinuation.End(context.UserId, CommandList.MoveToReview, context.MessageId);
                return;
            }

            currentDialog.AddToData(value.ToString());
            var message = await context.Client.SendTextMessageAsync(
                context.ChatId,
                await context.TranslateProvider.Get(Messages.Reviewer_EnterRequestForReview, context.LanguageId),
                cancellationToken: cancellationToken);
            currentDialog.AttachMessage(context.MessageId).AttachMessage(message.MessageId);
        }
    }

    private async Task ConnectToTeam(CommandContext context, Guid teamId, CancellationToken cancellationToken)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var team = await _teamRepository.Find(teamId, cancellationToken);
        if (team is { })
        {
            team.AddPlayer(context.LanguageId, context.UserId, context.UserName, context.UserLogin);
            await _teamRepository.Upsert(team, cancellationToken);
            await context.Client.SendTextMessageAsync(
                context.UserId,
                await context.TranslateProvider.Get(Messages.Reviewer_JoinToTeamSuccess, context.LanguageId, team.Name),
                cancellationToken: cancellationToken);
        }
        else
            await context.Client.SendTextMessageAsync(
                context.UserId,
                await context.TranslateProvider.Get(Messages.Reviewer_TeamNotFoundError, context.LanguageId),
                cancellationToken: cancellationToken);
    }

    private async Task MoveToState(
        Guid activeTask,
        Func<TaskForReview, CancellationToken, Task> action,
        CancellationToken cancellationToken)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));
        
        var targetTaskForReview = await _taskForReviewRepository.GetById(activeTask, cancellationToken);
        await action(targetTaskForReview, cancellationToken);
        await _taskForReviewRepository.Upsert(targetTaskForReview, cancellationToken);
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