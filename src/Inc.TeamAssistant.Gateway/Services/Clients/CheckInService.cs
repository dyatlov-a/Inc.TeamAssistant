using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class CheckInService : ICheckInService
{
    private readonly IMediator _mediator;

    public CheckInService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<GetMapsResult> GetMaps(Guid botId, CancellationToken token)
    {
        return await _mediator.Send(new GetMapsQuery(botId), token);
    }

    public async Task<GetLocationsResult> GetLocations(Guid mapId, CancellationToken token)
    {
        return await _mediator.Send(new GetLocationsQuery(mapId), token);
    }
}