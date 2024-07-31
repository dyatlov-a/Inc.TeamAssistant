using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Gateway.Services.ServerCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("open-graph")]
public sealed class OpenGraphController : ControllerBase
{
    private readonly OpenGraphService _openGraphService;

    public OpenGraphController(OpenGraphService openGraphService)
    {
        _openGraphService = openGraphService ?? throw new ArgumentNullException(nameof(openGraphService));
    }

    [HttpGet("create-card/{img}")]
    [OutputCache(PolicyName = CachePolicies.OpenGraphCachePolicy)]
    [ResponseCache(Duration = CachePolicies.OpenGraphCacheDurationInSeconds)]
    public async Task<IActionResult> Create(string img, string text, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(img);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        const string contentType = "image/jpeg";
        var stream = await _openGraphService.CreateCard(img, text, token);
        
        return File(stream, contentType);
    }
}