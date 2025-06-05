using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetRetroState;

internal sealed class SetRetroStateCommandHandler : IRequestHandler<SetRetroStateCommand>
{
    private readonly IRetroStage _retroStage;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;

    public SetRetroStateCommandHandler(
        IRetroStage retroStage,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _retroStage = retroStage ?? throw new ArgumentNullException(nameof(retroStage));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(SetRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var ticket = new RetroStageTicket(currentPerson.Id, command.Finished);
        
        _retroStage.Set(command.TeamId, ticket);

        await _eventSender.RetroStateChanged(command.TeamId, currentPerson.Id, command.Finished);
    }
}