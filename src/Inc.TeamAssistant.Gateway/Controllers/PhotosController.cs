using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("photos")]
public sealed class PhotosController : ControllerBase
{
    private readonly IPhotosRepository _photosRepository;

    public PhotosController(IPhotosRepository photosRepository)
    {
        _photosRepository = photosRepository ?? throw new ArgumentNullException(nameof(photosRepository));
    }

    [HttpGet("{personId}")]
    [OutputCache(PolicyName = OutputCachePolicies.Images)]
    [ResponseCache(Duration = 60 * 60)]
    public async Task<IActionResult> Get(long personId, CancellationToken token)
    {
        const string contentType = "image/jpeg";
        var photo = await _photosRepository.Find(personId, token);

        return photo is null
            ? File("imgs/user_stub.jpg", contentType)
            : File(photo.Data, contentType);
    }
}