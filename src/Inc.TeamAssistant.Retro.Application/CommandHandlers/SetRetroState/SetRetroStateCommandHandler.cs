using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetRetroState;

internal sealed class SetRetroStateCommandHandler : IRequestHandler<SetRetroStateCommand>
{
    private readonly IRetroStage _retroStage;
    private readonly IRetroEventSender _eventSender;

    public SetRetroStateCommandHandler(IRetroStage retroStage, IRetroEventSender eventSender)
    {
        _retroStage = retroStage ?? throw new ArgumentNullException(nameof(retroStage));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(SetRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var ticket = new RetroStageTicket(command.PersonId, command.Finished, command.HandRaised);
        
        _retroStage.Set(command.TeamId, ticket);

        await _eventSender.RetroStateChanged(
            command.TeamId,
            command.PersonId,
            command.Finished,
            command.HandRaised);
    }
}