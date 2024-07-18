using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("user-data")]
[Authorize]
public sealed class UserDataController : ControllerBase
{
    private readonly IDataEditor _dataEditor;

    public UserDataController(IDataEditor dataEditor)
    {
        _dataEditor = dataEditor ?? throw new ArgumentNullException(nameof(dataEditor));
    }

    [HttpGet("{dataId:guid}")]
    public async Task<IActionResult> Get(Guid dataId, CancellationToken token)
    {
        var data = await _dataEditor.Get(dataId, token);
        
        return Ok(data);
    }
    
    [HttpPost("{dataId:guid}")]
    public async Task<IActionResult> Attach(Guid dataId, [FromBody]string data, CancellationToken token)
    {
        await _dataEditor.Attach(dataId, data, token);
        
        return Ok();
    }
}