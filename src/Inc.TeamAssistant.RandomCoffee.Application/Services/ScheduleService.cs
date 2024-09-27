using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.SelectPairs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.RandomCoffee.Application.Services;

internal sealed class ScheduleService : BackgroundService
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IRandomCoffeeReader _reader;
    private readonly IHolidayService _holidayService;
    private readonly ILogger<ScheduleService> _logger;

    public ScheduleService(
        ICommandExecutor commandExecutor,
        IRandomCoffeeReader reader,
        IHolidayService holidayService,
        ILogger<ScheduleService> logger)
    {
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
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

            await Task.Delay(GlobalSettings.NotificationsDelay, token);
        }
    }

    private async Task Execute(CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var entries = await _reader.GetByDate(now, token);

        foreach (var entry in entries)
        {
            if (!await _holidayService.IsWorkTime(entry.BotId, now, token))
                continue;
            
            var messageContext = MessageContext.CreateFromBackground(entry.BotId, entry.ChatId);
                
            IDialogCommand command = entry.State switch
            {
                RandomCoffeeState.Waiting => new SelectPairsCommand(messageContext, entry.Id),
                RandomCoffeeState.Idle => new InviteForCoffeeCommand(messageContext, OnDemand: false),
                _ => throw new ArgumentOutOfRangeException(nameof(entry.State), entry.State, "State out of range.")
            };
                
            await _commandExecutor.Execute(command, token);
        }
    }
}