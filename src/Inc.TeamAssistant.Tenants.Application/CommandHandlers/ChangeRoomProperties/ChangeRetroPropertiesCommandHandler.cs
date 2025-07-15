using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Rooms;
using Inc.TeamAssistant.Tenants.Application.Common;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Domain;
using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.ChangeRoomProperties;

internal sealed class ChangeRetroPropertiesCommandHandler : IRequestHandler<ChangeRoomPropertiesCommand>
{
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    private readonly IRoomEventSender _eventSender;

    public ChangeRetroPropertiesCommandHandler(
        IRoomPropertiesProvider propertiesProvider,
        IPersonResolver personResolver,
        IRoomEventSender eventSender)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(ChangeRoomPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var currentProperties = await _propertiesProvider.Get(command.RoomId, token);
        var retroProperties = new RoomProperties(
            command.IsFacilitator == true ? currentPerson.Id : currentProperties.FacilitatorId,
            command.RetroTemplateId ?? currentProperties.RetroTemplateId,
            command.SurveyTemplateId ?? currentProperties.SurveyTemplateId,
            command.TimerDuration ?? currentProperties.TimerDuration,
            command.VoteCount ?? currentProperties.VoteCount,
            command.VoteByItemCount ?? currentProperties.VoteByItemCount,
            command.RetroType ?? currentProperties.RetroType);
        
        await _propertiesProvider.Set(command.RoomId, retroProperties, token);

        await _eventSender.PropertiesChanged(
            command.RoomId,
            RoomPropertiesConverter.ConvertTo(retroProperties));
    }
}