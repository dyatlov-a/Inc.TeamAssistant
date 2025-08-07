using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.MoveToNextRetroState;

internal sealed class MoveToNextRetroStateCommandHandler : IRequestHandler<MoveToNextRetroStateCommand>
{
    private readonly IRetroSessionRepository _repository;
    private readonly IRetroEventSender _eventSender;
    private readonly IVoteStore _voteStore;
    private readonly IPersonState _personState;

    public MoveToNextRetroStateCommandHandler(
        IRetroSessionRepository repository,
        IRetroEventSender eventSender,
        IVoteStore voteStore,
        IPersonState personState)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _voteStore = voteStore ?? throw new ArgumentNullException(nameof(voteStore));
        _personState = personState ?? throw new ArgumentNullException(nameof(personState));
    }

    public async Task Handle(MoveToNextRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var retroSession = await command.Id.Required(_repository.Find, token);
        
        retroSession.MoveToNextState();
        
        var votes = retroSession.State == RetroSessionState.Discussing
            ? _voteStore.Get(retroSession.Id)
            : [];

        await _repository.Update(retroSession, votes, token);

        _voteStore.Clear(retroSession.Id);
        _personState.Clear(RoomId.CreateForRetro(retroSession.RoomId));
        
        await _eventSender.RetroSessionChanged(RetroSessionConverter.ConvertTo(retroSession));
    }
}