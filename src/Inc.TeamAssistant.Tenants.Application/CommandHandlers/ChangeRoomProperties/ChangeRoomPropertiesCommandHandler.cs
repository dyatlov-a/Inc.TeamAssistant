using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.ChangeRoomProperties;

internal sealed class ChangeRoomPropertiesCommandHandler : IRequestHandler<ChangeRoomPropertiesCommand>
{
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;

    public ChangeRoomPropertiesCommandHandler(
        IRoomPropertiesProvider propertiesProvider,
        IPersonResolver personResolver)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(ChangeRoomPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var roomProperties = await _propertiesProvider.Get(command.RoomId, token);
        var currentPerson = _personResolver.GetCurrentPerson();
        var newRoomProperties = new RoomProperties(
            command.IsFacilitator == true ? currentPerson.Id : roomProperties.FacilitatorId,
            command.RetroTemplateId ?? roomProperties.RetroTemplateId,
            command.SurveyTemplateId ?? roomProperties.SurveyTemplateId,
            command.TimerDuration ?? roomProperties.TimerDuration,
            command.VoteCount ?? roomProperties.VoteCount,
            command.VoteByItemCount ?? roomProperties.VoteByItemCount,
            string.IsNullOrWhiteSpace(command.RetroType) ? roomProperties.RetroType : command.RetroType);
        
        await _propertiesProvider.Set(command.RoomId, newRoomProperties, token);
    }
}