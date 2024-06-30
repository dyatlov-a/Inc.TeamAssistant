using Inc.TeamAssistant.Gateway.Services.ServerCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SixLabors.Fonts;

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
    [OutputCache(PolicyName = "photos")]
    [ResponseCache(Duration = 60 * 60)]
    public async Task<IActionResult> Create(string img, string text, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(img);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        const string contentType = "image/jpeg";
        const string textFont = "Arial";
        const float fontSize = 32;
        const int padding = 30;
        
        var stream = await _openGraphService.CreateCard(
            img,
            text,
            textFont,
            fontSize,
            FontStyle.Regular,
            padding,
            token);
        
        return File(stream, contentType);
    }
}