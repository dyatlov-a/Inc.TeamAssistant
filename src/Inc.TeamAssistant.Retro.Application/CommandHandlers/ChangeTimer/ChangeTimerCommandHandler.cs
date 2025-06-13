using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeTimer;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeTimer;

internal sealed class ChangeTimerCommandHandler : IRequestHandler<ChangeTimerCommand>
{
    private readonly ITimerService _timerService;
    private readonly IRetroEventSender _eventSender;

    public ChangeTimerCommandHandler(ITimerService timerService, IRetroEventSender eventSender)
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