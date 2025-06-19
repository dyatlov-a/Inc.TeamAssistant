using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Queries.Common;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetRoom;

internal sealed class GetRoomQueryHandler : IRequestHandler<GetRoomQuery, GetRoomResult>
{
    private readonly ITenantRepository _repository;

    public GetRoomQueryHandler(ITenantRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task<GetRoomResult> Handle(GetRoomQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var room = await query.Id.Required(_repository.FindRoom, token);

        return new GetRoomResult(new RoomDto(room.Id, room.Name));
    }
}