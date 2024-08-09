using Inc.TeamAssistant.Connector.Model.Queries.GetPersonPhoto;
using Inc.TeamAssistant.Gateway.Configs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("photos")]
public sealed class PhotosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PhotosController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{personId}")]
    [ResponseCache(Duration = CachePolicies.UserAvatarClientCacheDurationInSeconds)]
    public async Task<IActionResult> Get(long personId, CancellationToken token)
    {
        const string contentType = "image/jpeg";
        var result = await _mediator.Send(new GetPersonPhotoQuery(personId), token);
        
        return result.Photo is null
            ? File("imgs/user_stub.jpg", contentType)
            : File(result.Photo, contentType);
    }
}