using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
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

    public async Task<ServiceResult<GetLocationsResult?>> GetLocations(Guid mapId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetLocationsQuery(mapId), cancellationToken);

            return result.Locations.Any()
                ? ServiceResult.Success((GetLocationsResult?)result)
                : ServiceResult.NotFound<GetLocationsResult?>();
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLocationsResult?>(ex.Message);
        }
    }
}