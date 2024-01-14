using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class NotificationsService : BackgroundService
{
    private readonly ITaskForReviewAccessor _accessor;
    private readonly IHolidayService _holidayService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITeamAccessor _teamAccessor;
    private readonly WorkdayOptions _options;
    private readonly TelegramBotClient _client;
    private readonly int _notificationsBatch;
    private readonly TimeSpan _notificationsDelay;

    public NotificationsService(
        ITaskForReviewAccessor accessor,
        IHolidayService holidayService,
        IServiceProvider serviceProvider,
        ITeamAccessor teamAccessor,
        WorkdayOptions options,
        string accessToken,
        int notificationsBatch,
        TimeSpan notificationsDelay)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(accessToken));
        
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _client = new(accessToken);
        _notificationsBatch = notificationsBatch;
        _notificationsDelay = notificationsDelay;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow;
            using var scope = _serviceProvider.CreateScope();
            var translateProvider = scope.ServiceProvider.GetRequiredService<ITranslateProvider>();

            if (await IsWorkTime(now, token))
            {
                var tasksForNotifications = await _accessor.GetTasksForNotifications(
                    now,
                    TaskForReviewStateRules.ActiveStates,
                    _notificationsBatch,
                    token);

                foreach (var task in tasksForNotifications)
                {
                    task.SetNextNotificationTime(_options.NotificationInterval);

                    var message = task.State switch
                    {
                        TaskForReviewState.New or TaskForReviewState.InProgress => await CreateNeedReviewMessage(translateProvider, task, token),
                        TaskForReviewState.OnCorrection => await CreateMoveToNextRoundMessage(translateProvider, task, token),
                        _ => throw new ArgumentOutOfRangeException($"Value {task.State} OutOfRange for {nameof(TaskForReviewState)}")
                    };

                    await _client.SendTextMessageAsync(
                        new(message.UserId),
                        message.Text,
                        replyMarkup: message.ReplyMarkup,
                        cancellationToken: token);
                }

                await _accessor.Update(tasksForNotifications, token);
            }

            await Task.Delay(_notificationsDelay, token);
        }
    }

    private async Task<(long UserId, string Text, IReplyMarkup ReplyMarkup)> CreateNeedReviewMessage(
        ITranslateProvider translateProvider,
        TaskForReview task,
        CancellationToken token)
    {
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (task is null)
            throw new ArgumentNullException(nameof(task));

        var reviewer = await _teamAccessor.FindPerson(task.ReviewerId, token);
        if (!reviewer.HasValue)
            throw new ApplicationException($"Reviewer {task.ReviewerId} was not found.");
        
        var buttons = new[]
        {
            InlineKeyboardButton.WithCallbackData(
                await translateProvider.Get(Messages.Reviewer_MoveToInProgress, reviewer.Value.LanguageId),
                $"{CommandList.MoveToInProgress}{task.Id:N}"),
            InlineKeyboardButton.WithCallbackData(
                await translateProvider.Get(Messages.Reviewer_MoveToAccept, reviewer.Value.LanguageId),
                $"{CommandList.Accept}{task.Id:N}"),
            InlineKeyboardButton.WithCallbackData(
                await translateProvider.Get(Messages.Reviewer_MoveToDecline, reviewer.Value.LanguageId),
                $"{CommandList.Decline}{task.Id:N}")
        };

        return (
            reviewer.Value.Id,
            await translateProvider.Get(Messages.Reviewer_NeedReview, reviewer.Value.LanguageId, task.Description),
            new InlineKeyboardMarkup(buttons));
    }
    
    private async Task<(long UserId, string Text, IReplyMarkup ReplyMarkup)> CreateMoveToNextRoundMessage(
        ITranslateProvider translateProvider,
        TaskForReview task,
        CancellationToken token)
    {
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (task is null)
            throw new ArgumentNullException(nameof(task));
        
        var owner = await _teamAccessor.FindPerson(task.OwnerId, token);
        if (!owner.HasValue)
            throw new ApplicationException($"Owner {task.OwnerId} was not found.");
        
        var buttons = new[]
        {
            InlineKeyboardButton.WithCallbackData(
                await translateProvider.Get(Messages.Reviewer_MoveToNextRound, owner.Value.LanguageId),
                $"{CommandList.MoveToNextRound}{task.Id:N}")
        };

        return (
            owner.Value.Id,
            await translateProvider.Get(Messages.Reviewer_ReviewDeclined, owner.Value.LanguageId, task.Description),
            new InlineKeyboardMarkup(buttons));
    }

    private async Task<bool> IsWorkTime(DateTimeOffset dateTimeOffset, CancellationToken token)
    {
        if (_options.WorkOnHoliday)
            return true;

        if (dateTimeOffset.TimeOfDay < _options.StartTimeUtc || dateTimeOffset.TimeOfDay >= _options.EndTimeUtc)
            return false;

        return await _holidayService.IsWorkday(DateOnly.FromDateTime(dateTimeOffset.DateTime), token);
    }
}