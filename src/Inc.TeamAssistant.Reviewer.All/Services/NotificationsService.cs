using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.Holidays;
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
                    _notificationsBatch,
                    stoppingToken);

                foreach (var tasksForNotification in tasksForNotifications)
                {
                    tasksForNotification.SetNextNotificationTime(_options.NotificationInterval);

                    var messageText = await translateProvider.Get(
                        Messages.Reviewer_NeedReview,
                        tasksForNotification.Reviewer.LanguageId,
                        tasksForNotification.Description,
                        $"{CommandList.Finish}_{tasksForNotification.Id:N}");
                    await _client.SendTextMessageAsync(
                        new(tasksForNotification.Reviewer.UserId),
                        messageText,
                        cancellationToken: stoppingToken);
                }

                await _accessor.Update(tasksForNotifications, stoppingToken);
            }

            await Task.Delay(_notificationsDelay, stoppingToken);
        }
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