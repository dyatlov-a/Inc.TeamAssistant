using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.RemoveRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;
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

    public async Task<GetAvailableRoomsResult> GetAvailableRooms(Guid? id, CancellationToken token)
    {
        return await _mediator.Send(new GetAvailableRoomsQuery(id), token);
    }

    public async Task<GetRoomResult> GetRoom(Guid id, CancellationToken token)
    {
        return await _mediator.Send(new GetRoomQuery(id), token);
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

    public async Task RemoveRoom(Guid teamId, CancellationToken token)
    {
        await _mediator.Send(new RemoveRoomCommand(teamId), token);
    }
}