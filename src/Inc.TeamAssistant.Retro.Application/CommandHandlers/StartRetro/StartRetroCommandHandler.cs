using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.StartRetro;

internal sealed class StartRetroCommandHandler : IRequestHandler<StartRetroCommand>
{
    private readonly IRetroSessionRepository _repository;
    private readonly IRetroEventSender _eventSender;
    private readonly IPersonState _personState;

    public StartRetroCommandHandler(
        IRetroSessionRepository repository,
        IRetroEventSender eventSender,
        IPersonState personState)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _personState = personState ?? throw new ArgumentNullException(nameof(personState));
    }

    public async Task Handle(StartRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var retroSession = new RetroSession(Guid.CreateVersion7(), command.RoomId, DateTimeOffset.UtcNow);

        await _repository.Create(retroSession, token);
        
        _personState.Clear(RoomId.CreateForRetro(retroSession.RoomId));

        await _eventSender.RetroSessionChanged(RetroSessionConverter.ConvertTo(retroSession));
    }
}