using Inc.TeamAssistant.Constructor.Model.Commands.CreateCalendar;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("calendars")]
[Authorize]
public sealed class CalendarsController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    public CalendarsController(ICalendarService calendarService)
    {
        _calendarService = calendarService ?? throw new ArgumentNullException(nameof(calendarService));
    }

    [HttpGet("{ownerId:long}/owner")]
    public async Task<IActionResult> Get(long ownerId, CancellationToken token)
    {
        return Ok(await _calendarService.GetCalendarByOwner(ownerId, token));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        return Ok(await _calendarService.Create(command, token));
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody]UpdateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        return Ok(await _calendarService.Update(command, token));
    }
}