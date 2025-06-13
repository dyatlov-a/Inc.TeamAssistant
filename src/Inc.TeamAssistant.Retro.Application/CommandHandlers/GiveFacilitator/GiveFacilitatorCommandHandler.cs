using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.GiveFacilitator;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.GiveFacilitator;

internal sealed class GiveFacilitatorCommandHandler : IRequestHandler<GiveFacilitatorCommand>
{
    private readonly IRetroPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;

    public GiveFacilitatorCommandHandler(
        IRetroPropertiesProvider propertiesProvider,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(GiveFacilitatorCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        
        await _propertiesProvider.Set(command.RoomId, new (){ FacilitatorId = currentPerson.Id }, token);

        await _eventSender.FacilitatorChanged(command.RoomId, currentPerson.Id);
    }
}