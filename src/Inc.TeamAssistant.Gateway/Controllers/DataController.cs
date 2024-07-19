using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("data")]
[Authorize]
public sealed class DataController : ControllerBase
{
    private readonly IDataEditor _dataEditor;

    public DataController(IDataEditor dataEditor)
    {
        _dataEditor = dataEditor ?? throw new ArgumentNullException(nameof(dataEditor));
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        var data = await _dataEditor.Get(key, token);
        
        return Ok(data);
    }
    
    [HttpPost("{key}")]
    public async Task<IActionResult> Attach(string key, [FromBody]string data, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        await _dataEditor.Attach(key, data, token);
        
        return Ok();
    }
    
    [HttpDelete("{key}")]
    public async Task<IActionResult> Detach(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        await _dataEditor.Detach(key, token);
        
        return Ok();
    }
}