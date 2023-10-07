using Inc.TeamAssistant.Appraiser.Model;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

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