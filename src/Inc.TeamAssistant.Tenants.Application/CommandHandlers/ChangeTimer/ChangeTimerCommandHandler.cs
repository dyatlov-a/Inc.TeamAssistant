using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Commands.ChangeTimer;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.ChangeTimer;

internal sealed class ChangeTimerCommandHandler : IRequestHandler<ChangeTimerCommand>
{
    private readonly ITimerService _timerService;
    private readonly IRoomEventSender _eventSender;

    public ChangeTimerCommandHandler(ITimerService timerService, IRoomEventSender eventSender)
    {
        _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(ChangeTimerCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        if (command.Duration.HasValue)
            _timerService.Start(command.RoomId, command.Duration.Value);
        else
            _timerService.Stop(command.RoomId);

        await _eventSender.TimerChanged(command.RoomId, command.Duration);
    }
}