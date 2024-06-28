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
    [OutputCache]
    [ResponseCache(Duration = 60 * 60)]
    public async Task<IActionResult> Get(long personId, CancellationToken token)
    {
        var photo = await _photosRepository.Find(personId, token);
        if (photo is not null)
            return File(photo.Data, "image/jpeg");
        
        return File("imgs/user_stub.jpg", "image/jpeg");
    }
}