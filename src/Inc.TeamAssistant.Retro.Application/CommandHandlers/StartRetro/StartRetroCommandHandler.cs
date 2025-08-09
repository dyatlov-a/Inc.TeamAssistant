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
    private readonly IOnlinePersonStore _onlinePersonStore;

    public StartRetroCommandHandler(
        IRetroSessionRepository repository,
        IRetroEventSender eventSender,
        IOnlinePersonStore onlinePersonStore)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
    }

    public async Task Handle(StartRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var retroSession = new RetroSession(Guid.CreateVersion7(), command.RoomId, DateTimeOffset.UtcNow);

        await _repository.Create(retroSession, token);
        
        _onlinePersonStore.ClearTickets(RoomId.CreateForRetro(retroSession.RoomId));

        await _eventSender.RetroSessionChanged(RetroSessionConverter.ConvertTo(retroSession));
    }
}