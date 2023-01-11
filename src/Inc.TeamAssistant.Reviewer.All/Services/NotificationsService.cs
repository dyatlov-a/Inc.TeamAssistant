using System.Text;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.Holidays;
using Inc.TeamAssistant.Reviewer.All.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.All.Services;

internal sealed class NotificationsService : BackgroundService
{
    private readonly ITaskForReviewAccessor _accessor;
    private readonly IHolidayService _holidayService;
    private readonly IServiceProvider _serviceProvider;
    private readonly WorkdayOptions _options;
    private readonly TelegramBotClient _client;
    private readonly int _notificationsBatch;
    private readonly TimeSpan _notificationsDelay;

    public NotificationsService(
        ITaskForReviewAccessor accessor,
        IHolidayService holidayService,
        IServiceProvider serviceProvider,
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
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _client = new(accessToken);
        _notificationsBatch = notificationsBatch;
        _notificationsDelay = notificationsDelay;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow;
            using var scope = _serviceProvider.CreateScope();
            var translateProvider = scope.ServiceProvider.GetRequiredService<ITranslateProvider>();

            if (await IsWorkTime(now, stoppingToken))
            {
                var tasksForNotifications = await _accessor.GetTasksForNotifications(
                    now,
                    TaskForReviewStateRules.ActiveStates,
                    _notificationsBatch,
                    stoppingToken);

                foreach (var task in tasksForNotifications)
                {
                    task.SetNextNotificationTime(_options.NotificationInterval);

                    var message = task.State switch
                    {
                        TaskForReviewState.InProgress => await CreateNeedReviewMessage(translateProvider, task),
                        TaskForReviewState.OnCorrection => await CreateMoveToNextRoundMessage(translateProvider, task),
                        _ => throw new ArgumentOutOfRangeException($"Value {task.State} OutOfRange for {nameof(TaskForReviewState)}")
                    };

                    await _client.SendTextMessageAsync(new(message.UserId), message.Text, cancellationToken: stoppingToken);
                }

                await _accessor.Update(tasksForNotifications, stoppingToken);
            }

            await Task.Delay(_notificationsDelay, stoppingToken);
        }
    }

    private async Task<(long UserId, string Text)> CreateNeedReviewMessage(
        ITranslateProvider translateProvider,
        TaskForReview task)
    {
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (task is null)
            throw new ArgumentNullException(nameof(task));
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await translateProvider.Get(
            Messages.Reviewer_NeedReview,
            task.Reviewer.LanguageId,
            task.Description));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine($"1. {CommandList.Accept}_{task.Id:N}");
        messageBuilder.AppendLine();
        messageBuilder.AppendLine($"2. {CommandList.Decline}_{task.Id:N}");

        return (task.Reviewer.UserId, messageBuilder.ToString());
    }
    
    private async Task<(long UserId, string Text)> CreateMoveToNextRoundMessage(
        ITranslateProvider translateProvider,
        TaskForReview task)
    {
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (task is null)
            throw new ArgumentNullException(nameof(task));
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await translateProvider.Get(
            Messages.Reviewer_ReviewDeclined,
            task.Owner.LanguageId,
            task.Description));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine($"1. {CommandList.MoveToNextRound}_{task.Id:N}");

        return (task.Owner.UserId, messageBuilder.ToString());
    }

    private async Task<bool> IsWorkTime(DateTimeOffset dateTimeOffset, CancellationToken cancellationToken)
    {
        if (_options.WorkOnHoliday)
            return true;

        if (dateTimeOffset.TimeOfDay < _options.StartTimeUtc || dateTimeOffset.TimeOfDay >= _options.EndTimeUtc)
            return false;

        return await _holidayService.IsWorkday(DateOnly.FromDateTime(dateTimeOffset.DateTime), cancellationToken);
    }
}