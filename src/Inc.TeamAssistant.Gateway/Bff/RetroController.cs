using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("retro/items")]
[Authorize]
public sealed class RetroController : ControllerBase
{
    private readonly IRetroService _retroService;

    public RetroController(IRetroService retroService)
    {
        _retroService = retroService ?? throw new ArgumentNullException(nameof(retroService));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateItem([FromBody]CreateRetroItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return Ok(await _retroService.CreateRetroItem(command, CancellationToken.None));
    }
}