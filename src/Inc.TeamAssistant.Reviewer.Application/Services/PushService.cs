using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.SendPush;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class PushService : BackgroundService
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ITaskForReviewReader _reader;
    private readonly IHolidayService _holidayService;
    private readonly ReviewerOptions _options;
    private readonly ILogger<PushService> _logger;

    public PushService(
        ICommandExecutor commandExecutor,
        ITaskForReviewReader reader,
        IHolidayService holidayService,
        ReviewerOptions options,
        ILogger<PushService> logger)
    {
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Push(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on send notifications by tasks");
            }

            await Task.Delay(_options.NotificationsDelay, token);
        }
    }

    private async Task Push(CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;

        if (await _holidayService.IsWorkTime(now, token))
        {
            var tasksForNotifications = await _reader.GetTasksForNotifications(
                now,
                TaskForReviewStateRules.ActiveStates,
                _options.NotificationsBatch,
                token);

            foreach (var task in tasksForNotifications)
                await _commandExecutor.Execute(
                    new SendPushCommand(MessageContext.CreateIdle(task.BotId, task.ChatId), task.Id),
                    token);
        }
    }
}