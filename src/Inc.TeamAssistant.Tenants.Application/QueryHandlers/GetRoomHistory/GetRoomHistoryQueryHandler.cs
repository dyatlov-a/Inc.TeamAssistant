using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomHistory;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetRoomHistory;

internal sealed class GetRoomHistoryQueryHandler : IRequestHandler<GetRoomHistoryQuery, GetRoomHistoryResult>
{
    private readonly ITenantReader _reader;

    public GetRoomHistoryQueryHandler(ITenantReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetRoomHistoryResult> Handle(GetRoomHistoryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var firstRetro = new DateTimeOffset(2025, 8, 19, 0, 0, 0, TimeSpan.Zero);
        
        var items = await _reader.GetRoomHistory(query.RoomId, firstRetro, token);

        return new GetRoomHistoryResult(items);
    }
}