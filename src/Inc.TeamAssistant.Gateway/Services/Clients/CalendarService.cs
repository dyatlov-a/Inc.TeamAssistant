using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateCalendar;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class CalendarService : ICalendarService
{
    private readonly IMediator _mediator;

    public CalendarService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    public async Task<ServiceResult<GetCalendarByOwnerResult?>> GetCalendarByOwner(
        long ownerId,
        CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetCalendarByOwnerQuery(ownerId), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetCalendarByOwnerResult?>(ex.Message);
        }
    }

    public async Task Create(CreateCalendarCommand command, CancellationToken token)
    {
        await _mediator.Send(command, token);
    }
}