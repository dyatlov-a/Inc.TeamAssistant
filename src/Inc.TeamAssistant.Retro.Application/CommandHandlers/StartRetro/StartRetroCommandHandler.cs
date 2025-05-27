using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.StartRetro;

internal sealed class StartRetroCommandHandler : IRequestHandler<StartRetroCommand>
{
    private readonly IRetroSessionRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;

    public StartRetroCommandHandler(
        IRetroSessionRepository repository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(StartRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var retroSession = new RetroSession(
            Guid.CreateVersion7(),
            command.TeamId,
            DateTimeOffset.UtcNow,
            currentPerson.Id);

        await _repository.Create(retroSession, token);

        await _eventSender.RetroSessionChanged(RetroSessionConverter.ConvertTo(retroSession));
    }
}