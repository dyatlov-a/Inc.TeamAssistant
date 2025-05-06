using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroCardPool;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroCardPool;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("retro")]
[Authorize]
public sealed class RetroController : ControllerBase
{
    private readonly IRetroService _retroService;

    public RetroController(IRetroService retroService)
    {
        _retroService = retroService ?? throw new ArgumentNullException(nameof(retroService));
    }

    [HttpPost("card-pool")]
    public async Task<IActionResult> CreateRetroCardPool(CreateRetroCardPoolCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return Ok(await _retroService.CreateRetroCardPool(command, CancellationToken.None));
    }
    
    [HttpPut("card-pool")]
    public async Task<IActionResult> UpdateRetroCardPool(UpdateRetroCardPoolCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _retroService.UpdateRetroCardPool(command, CancellationToken.None);
        
        return Ok();
    }
}