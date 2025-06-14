using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeRoomProperties;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeRoomProperties;

internal sealed class ChangeRoomPropertiesCommandHandler : IRequestHandler<ChangeRoomPropertiesCommand>
{
    private readonly IRetroPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;

    public ChangeRoomPropertiesCommandHandler(
        IRetroPropertiesProvider propertiesProvider,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(ChangeRoomPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var roomProperties = new RetroProperties
        {
            FacilitatorId = command.IsFacilitator == true ? currentPerson.Id : null,
            TemplateId = command.TemplateId,
            TimerDuration = command.TimerDuration,
            VoteCount = command.VoteCount
        };
        
        await _propertiesProvider.Set(command.RoomId, roomProperties, token);

        await _eventSender.RoomPropertiesChanged(command.RoomId);
    }
}