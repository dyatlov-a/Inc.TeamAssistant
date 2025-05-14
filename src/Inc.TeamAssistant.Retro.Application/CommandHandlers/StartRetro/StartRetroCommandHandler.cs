using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.StartRetro;

internal sealed class StartRetroCommandHandler : IRequestHandler<StartRetroCommand, StartRetroResult>
{
    private readonly IRetroRepository _retroRepository;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _retroEventSender;

    public StartRetroCommandHandler(
        IRetroRepository retroRepository,
        IPersonResolver personResolver,
        IRetroEventSender retroEventSender)
    {
        _retroRepository = retroRepository ?? throw new ArgumentNullException(nameof(retroRepository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _retroEventSender = retroEventSender ?? throw new ArgumentNullException(nameof(retroEventSender));
    }

    public async Task<StartRetroResult> Handle(StartRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var retroSession = new RetroSession(
            Guid.CreateVersion7(),
            command.TeamId,
            DateTimeOffset.UtcNow,
            currentPerson.Id);

        await _retroRepository.Create(retroSession, token);

        await _retroEventSender.RetroSessionChanged(RetroSessionConverter.ConvertTo(retroSession));

        return new StartRetroResult(retroSession.Id);
    }
}