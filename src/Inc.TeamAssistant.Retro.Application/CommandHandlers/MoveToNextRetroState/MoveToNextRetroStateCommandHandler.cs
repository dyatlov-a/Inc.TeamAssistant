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

    public MoveToNextRetroStateCommandHandler(
        IRetroSessionRepository repository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(MoveToNextRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _personResolver.GetCurrentPerson();
        var retroSession = await command.Id.Required(_repository.Find, token);

        // TODO: Impl next state
        if (retroSession.State == RetroSessionState.Prioritizing)
            return;
        
        retroSession
            .EnsureRights(currentPerson.Id)
            .MoveToNextState();

        await _repository.Update(retroSession, token);
        
        await _eventSender.RetroSessionChanged(RetroSessionConverter.ConvertTo(retroSession));
    }
}