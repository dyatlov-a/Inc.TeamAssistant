using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.MoveToNextRetroState;

internal sealed class MoveToNextRetroStateCommandHandler : IRequestHandler<MoveToNextRetroStateCommand>
{
    private readonly IRetroSessionRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;
    private readonly IVoteStore _voteStore;

    public MoveToNextRetroStateCommandHandler(
        IRetroSessionRepository repository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender,
        IVoteStore voteStore)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _voteStore = voteStore ?? throw new ArgumentNullException(nameof(voteStore));
    }

    public async Task Handle(MoveToNextRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _personResolver.GetCurrentPerson();
        var retroSession = await command.Id.Required(_repository.Find, token);
        
        retroSession
            .EnsureRights(currentPerson.Id)
            .MoveToNextState();
        
        var votes = retroSession.State == RetroSessionState.Discussing
            ? _voteStore.Get(retroSession.Id)
            : [];

        await _repository.Update(retroSession, votes, token);

        _voteStore.Clear(retroSession.Id);
        
        await _eventSender.RetroSessionChanged(RetroSessionConverter.ConvertTo(retroSession));
    }
}