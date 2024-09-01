using Inc.TeamAssistant.Constructor.Model.Commands.CreateCalendar;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
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
    
    public async Task<GetCalendarByOwnerResult?> GetCalendarByOwner(CancellationToken token)
    {
        return await _mediator.Send(new GetCalendarByOwnerQuery(), token);
    }

    public async Task<Guid> Create(CreateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return await _mediator.Send(command, token);
    }

    public async Task<Guid> Update(UpdateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return await _mediator.Send(command, token);
    }
}