using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroAssessment;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
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
    
    [HttpGet("{roomId:Guid}/state")]
    public async Task<IActionResult> Get(Guid roomId, CancellationToken token)
    {
        return Ok(await _retroService.GetRetroState(roomId, token));
    }
    
    [HttpPost]
    public async Task<IActionResult> StartRetro([FromBody]StartRetroCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _retroService.StartRetro(command, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> MoveToNextRetroState([FromBody]MoveToNextRetroStateCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _retroService.MoveToNextRetroState(command, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpGet("{roomId:Guid}/actions")]
    public async Task<IActionResult> GetActionItems(Guid roomId, CancellationToken token)
    {
        return Ok(await _retroService.GetActionItems(roomId, token));
    }
    
    [HttpPut("actions")]
    public async Task<IActionResult> ChangeActionItem([FromBody]ChangeActionItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _retroService.ChangeActionItem(command, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpGet("{sessionId:Guid}/assessments")]
    public async Task<IActionResult> GetAssessments(Guid sessionId, CancellationToken token)
    {
        return Ok(await _retroService.GetRetroAssessment(sessionId, token));
    }
    
    [HttpPut("assessments")]
    public async Task<IActionResult> SetAssessment([FromBody]SetRetroAssessmentCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _retroService.SetRetroAssessment(command, CancellationToken.None);
        
        return Ok();
    }

    [HttpPut("properties")]
    public async Task<IActionResult> ChangeRetroProperties([FromBody]ChangeRetroPropertiesCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _retroService.ChangeRetroProperties(command, CancellationToken.None);
        
        return Ok();
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates(CancellationToken token)
    {
        return Ok(await _retroService.GetRetroTemplates(token));
    }
}