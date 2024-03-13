using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.SendNotification;
using Microsoft.Extensions.Hosting;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class NotificationsService : BackgroundService
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ITaskForReviewReader _reader;
    private readonly IHolidayService _holidayService;
    private readonly WorkdayOptions _options;
    private readonly int _notificationsBatch;
    private readonly TimeSpan _notificationsDelay;

    public NotificationsService(
        ICommandExecutor commandExecutor,
        ITaskForReviewReader reader,
        IHolidayService holidayService,
        WorkdayOptions options,
        int notificationsBatch,
        TimeSpan notificationsDelay)
    {
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _notificationsBatch = notificationsBatch;
        _notificationsDelay = notificationsDelay;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow;

            if (await IsWorkTime(now, token))
            {
                var tasksForNotifications = await _reader.GetTasksForNotifications(
                    now,
                    TaskForReviewStateRules.ActiveStates,
                    _notificationsBatch,
                    token);

                foreach (var task in tasksForNotifications)
                    await _commandExecutor.Execute(
                        new SendNotificationCommand(MessageContext.CreateIdle(task.BotId, task.ChatId), task.Id),
                        token);
            }

            await Task.Delay(_notificationsDelay, token);
        }
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