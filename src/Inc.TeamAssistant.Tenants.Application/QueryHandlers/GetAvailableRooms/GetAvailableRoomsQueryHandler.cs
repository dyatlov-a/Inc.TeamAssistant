using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Queries.Common;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetAvailableRooms;

internal sealed class GetAvailableRoomsQueryHandler : IRequestHandler<GetAvailableRoomsQuery, GetAvailableRoomsResult>
{
    private readonly ITenantReader _tenantReader;
    private readonly IPersonResolver _personResolver;

    public GetAvailableRoomsQueryHandler(ITenantReader tenantReader, IPersonResolver personResolver)
    {
        _tenantReader = tenantReader ?? throw new ArgumentNullException(nameof(tenantReader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task<GetAvailableRoomsResult> Handle(GetAvailableRoomsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var person = _personResolver.GetCurrentPerson();
        var rooms = await _tenantReader.GetAvailableRooms(query.RoomId, person.Id, token);
        
        var results = rooms
            .Select(t => new RoomDto(t.Id, t.Name))
            .OrderBy(t => t.Name)
            .ToArray();
        
        return new GetAvailableRoomsResult(results);
    }
}