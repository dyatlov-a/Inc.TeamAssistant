using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.SendPush;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class PushBackgroundService : BackgroundService
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ITaskForReviewReader _reader;
    private readonly IHolidayService _holidayService;
    private readonly IBotAccessor _botAccessor;
    private readonly ILogger<PushBackgroundService> _logger;

    public PushBackgroundService(
        ICommandExecutor commandExecutor,
        ITaskForReviewReader reader,
        IHolidayService holidayService,
        IBotAccessor botAccessor,
        ILogger<PushBackgroundService> logger)
    {
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
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

            await Task.Delay(GlobalSettings.NotificationsDelay, token);
        }
    }

    private async Task Push(CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var tasksForNotifications = await _reader.GetTasksForNotifications(
            now,
            TaskForReviewStateRules.ActiveStates,
            token);

        foreach (var task in tasksForNotifications)
        {
            if (!await _holidayService.IsWorkTime(task.BotId, now, token))
                continue;
            
            var botContext = await _botAccessor.GetBotContext(task.BotId, token);
            var messageContext = MessageContext.CreateFromBackground(botContext, task.ChatId);
                
            await _commandExecutor.Execute(new SendPushCommand(messageContext, task.Id), token);
        }
    }
}