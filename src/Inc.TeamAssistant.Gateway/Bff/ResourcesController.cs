using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("resources")]
public sealed class ResourcesController : ControllerBase
{
    private readonly IMessageProvider _messageProvider;

    public ResourcesController(IMessageProvider messageProvider)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _messageProvider.Get());
}