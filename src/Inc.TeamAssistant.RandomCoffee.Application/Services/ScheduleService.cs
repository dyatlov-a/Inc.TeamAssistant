using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives;
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
    private readonly RandomCoffeeOptions _options;
    private readonly ILogger<ScheduleService> _logger;

    public ScheduleService(
        ICommandExecutor commandExecutor,
        IRandomCoffeeReader reader,
        IHolidayService holidayService,
        RandomCoffeeOptions options,
        ILogger<ScheduleService> logger)
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
                await Execute(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on schedule RandomCoffee");
            }

            await Task.Delay(_options.ScheduleDelay, token);
        }
    }

    private async Task Execute(CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;

        if (await _holidayService.IsWorkTime(now, token) && now.TimeOfDay > _options.NotificationTime)
        {
            var date = DateOnly.FromDateTime(now.Date);
            var randomCoffeeEntries = await _reader.GetByDate(date, token);

            foreach (var randomCoffeeEntry in randomCoffeeEntries)
            {
                var messageContext = MessageContext.CreateIdle(randomCoffeeEntry.BotId, randomCoffeeEntry.ChatId);
                
                IDialogCommand command = randomCoffeeEntry.State switch
                {
                    RandomCoffeeState.Waiting => new SelectPairsCommand(messageContext, randomCoffeeEntry.Id),
                    RandomCoffeeState.Idle => new InviteForCoffeeCommand(messageContext, OnDemand: false),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                await _commandExecutor.Execute(command, token);
            }
        }
    }
}