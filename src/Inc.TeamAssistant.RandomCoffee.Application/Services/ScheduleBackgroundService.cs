using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.RepeatMeeting;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.SelectPairs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.RandomCoffee.Application.Services;

internal sealed class ScheduleBackgroundService : BackgroundService
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IRandomCoffeeReader _reader;
    private readonly IHolidayService _holidayService;
    private readonly IBotAccessor _botAccessor;
    private readonly ILogger<ScheduleBackgroundService> _logger;

    public ScheduleBackgroundService(
        ICommandExecutor commandExecutor,
        IRandomCoffeeReader reader,
        IHolidayService holidayService,
        IBotAccessor botAccessor,
        ILogger<ScheduleBackgroundService> logger)
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
                await Execute(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on schedule RandomCoffee");
            }

            await Task.Delay(GlobalResources.Settings.NotificationsDelay, token);
        }
    }

    private async Task Execute(CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var entries = await _reader.GetByDate(RandomCoffeeStateRules.ActiveStates, now, token);

        foreach (var entry in entries)
        {
            if (!await _holidayService.IsWorkTime(entry.BotId, now, token))
                continue;
            
            var botContext = await _botAccessor.GetBotContext(entry.BotId, token);
            var messageContext = MessageContext.CreateFromBackground(botContext, entry.ChatId);
                
            IDialogCommand command = entry.State switch
            {
                RandomCoffeeState.Waiting => new SelectPairsCommand(messageContext, entry.Id),
                RandomCoffeeState.Idle => new RepeatMeetingCommand(messageContext),
                _ => throw new ArgumentOutOfRangeException(nameof(entry.State), entry.State, "State out of range.")
            };
                
            await _commandExecutor.Execute(command, token);
        }
    }
}