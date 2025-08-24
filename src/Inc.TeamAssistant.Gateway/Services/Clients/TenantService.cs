using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.RemoveRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomHistory;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class TenantService : ITenantService
{
    private readonly IMediator _mediator;

    public TenantService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<GetAvailableRoomsResult> GetAvailableRooms(Guid? roomId, CancellationToken token)
    {
        return await _mediator.Send(new GetAvailableRoomsQuery(roomId), token);
    }

    public async Task<GetRoomResult> GetRoom(Guid roomId, CancellationToken token)
    {
        return await _mediator.Send(new GetRoomQuery(roomId), token);
    }

    public async Task<GetRoomPropertiesResult> GetRoomProperties(Guid roomId, CancellationToken token)
    {
        return await _mediator.Send(new GetRoomPropertiesQuery(roomId), token);
    }

    public async Task<GetRoomHistoryResult> GetRoomHistory(Guid roomId, CancellationToken token = default)
    {
        return await _mediator.Send(new GetRoomHistoryQuery(roomId), token);
    }

    public async Task ChangeRoomProperties(ChangeRoomPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _mediator.Send(command, token);
    }

    public async Task<CreateRoomResult> CreateRoom(CreateRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await _mediator.Send(command, token);
    }

    public async Task UpdateRoom(UpdateRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }

    public async Task RemoveRoom(Guid roomId, CancellationToken token)
    {
        await _mediator.Send(new RemoveRoomCommand(roomId), token);
    }
}