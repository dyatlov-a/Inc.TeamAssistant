using System.Text.Json;
using Inc.TeamAssistant.Common.Messages;
using Inc.TeamAssistant.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Appraiser.Backend.Controllers;

[ApiController]
[Route("resources")]
public sealed class ResourcesController : ControllerBase, IMessageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ResourcesController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
    }

    [HttpGet]
    public async Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, Dictionary<string, string>>();

        foreach (var languageId in Settings.LanguageIds)
        {
            var language = Path.Combine(_webHostEnvironment.WebRootPath, "langs", $"{languageId.Value}.json");
            var resourcesAsString = await System.IO.File.ReadAllTextAsync(language, cancellationToken: cancellationToken);
            var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(resourcesAsString);

            result.Add(languageId.Value, resources!);
        }

        return ServiceResult.Success(result);
    }
}