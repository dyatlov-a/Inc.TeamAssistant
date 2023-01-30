using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.CheckIn.Primitives;
using Inc.TeamAssistant.Primitives;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.CheckIn.API.Controllers;

[ApiController]
[Route("locations")]
public sealed class LocationsController : ControllerBase, ICheckInService
{
    private readonly IMediator _mediator;

    public LocationsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    public async Task<ServiceResult<AddLocationToMapResult>> AddLocationToMap(
        AddLocationToMapCommand command,
        CancellationToken cancellationToken)
    {
        return ServiceResult.Success(await _mediator.Send(command, cancellationToken));
    }

    [HttpGet("{mapId}")]
    public async Task<ServiceResult<GetLocationsResult>> GetLocations(Guid mapId, CancellationToken cancellationToken)
    {
        return ServiceResult.Success(await _mediator.Send(new GetLocationsQuery(new MapId(mapId)), cancellationToken));
    }
}