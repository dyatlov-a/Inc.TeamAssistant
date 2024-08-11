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
}