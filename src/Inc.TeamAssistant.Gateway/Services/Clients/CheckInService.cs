using Inc.TeamAssistant.Appraiser.Model.Common;
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

    public async Task<ServiceResult<GetMapsResult>> GetMaps(Guid botId, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetMapsQuery(botId), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetMapsResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetLocationsResult?>> GetLocations(Guid mapId, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetLocationsQuery(mapId), token);

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