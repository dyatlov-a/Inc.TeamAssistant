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
    private readonly IRetroStage _retroStage;

    public StartRetroCommandHandler(
        IRetroSessionRepository repository,
        IRetroEventSender eventSender,
        IRetroStage retroStage)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _retroStage = retroStage ?? throw new ArgumentNullException(nameof(retroStage));
    }

    public async Task Handle(StartRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var retroSession = new RetroSession(Guid.CreateVersion7(), command.RoomId, DateTimeOffset.UtcNow);

        await _repository.Create(retroSession, token);
        
        _retroStage.Clear(retroSession.RoomId);

        await _eventSender.RetroSessionChanged(RetroSessionConverter.ConvertTo(retroSession));
    }
}