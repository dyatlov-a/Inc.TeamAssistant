using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

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

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken token)
    {
        return Ok(await _calendarService.GetCalendarByOwner(token));
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody]UpdateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        return Ok(await _calendarService.Update(command, token));
    }
}