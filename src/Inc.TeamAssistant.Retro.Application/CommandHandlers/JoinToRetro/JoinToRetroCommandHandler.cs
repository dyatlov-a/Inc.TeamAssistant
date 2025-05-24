using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.JoinToRetro;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.JoinToRetro;

internal sealed class JoinToRetroCommandHandler : IRequestHandler<JoinToRetroCommand>
{
    private readonly IOnlinePersonStore _store;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender; 

    public JoinToRetroCommandHandler(
        IOnlinePersonStore store,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(JoinToRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var persons = _store.JoinToTeam(command.ConnectionId, command.TeamId, currentPerson);
        
        await _eventSender.PersonsChanged(command.TeamId, persons);
    }
}