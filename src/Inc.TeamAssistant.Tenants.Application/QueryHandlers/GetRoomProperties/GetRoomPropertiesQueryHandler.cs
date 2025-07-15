using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Rooms;
using Inc.TeamAssistant.Tenants.Application.Common;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetRoomProperties;

internal sealed class GetRoomPropertiesQueryHandler : IRequestHandler<GetRoomPropertiesQuery, GetRoomPropertiesResult>
{
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly ITimerService _timerService;
    private readonly IPersonResolver _personResolver;

    public GetRoomPropertiesQueryHandler(
        IRoomPropertiesProvider propertiesProvider,
        ITimerService timerService,
        IPersonResolver personResolver)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task<GetRoomPropertiesResult> Handle(GetRoomPropertiesQuery query, CancellationToken token)
    {
        var properties = await _propertiesProvider.Get(query.RoomId, token);
        var currentTimer = _timerService.TryGetValue(query.RoomId);
        var currentPerson = _personResolver.GetCurrentPerson();
        var isFacilitator = currentPerson.Id == properties.FacilitatorId;

        return new GetRoomPropertiesResult(
            RoomPropertiesConverter.ConvertTo(properties),
            isFacilitator,
            currentTimer);
    }
}