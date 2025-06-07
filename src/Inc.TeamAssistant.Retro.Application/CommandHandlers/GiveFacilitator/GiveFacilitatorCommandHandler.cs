using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.GiveFacilitator;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.GiveFacilitator;

internal sealed class GiveFacilitatorCommandHandler : IRequestHandler<GiveFacilitatorCommand>
{
    private readonly IFacilitatorProvider _facilitatorProvider;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroSessionRepository _repository;
    private readonly IRetroEventSender _eventSender;

    public GiveFacilitatorCommandHandler(
        IFacilitatorProvider facilitatorProvider,
        IPersonResolver personResolver,
        IRetroSessionRepository repository,
        IRetroEventSender eventSender)
    {
        _facilitatorProvider = facilitatorProvider ?? throw new ArgumentNullException(nameof(facilitatorProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(GiveFacilitatorCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        
        _facilitatorProvider.Set(command.TeamId, currentPerson.Id);

        if (command.RetroSessionId.HasValue)
        {
            var retroSession = await command.RetroSessionId.Value.Required(_repository.Find, token);

            await _repository.Update(
                retroSession.ChangeFacilitator(currentPerson.Id),
                voteTickets: [],
                token);
        }

        await _eventSender.FacilitatorChanged(command.TeamId, currentPerson.Id);
    }
}