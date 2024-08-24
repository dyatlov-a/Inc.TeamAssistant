using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetPersonPhoto;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetPersonPhoto;

internal sealed class GetPersonPhotoQueryHandler : IRequestHandler<GetPersonPhotoQuery, GetPersonPhotoResult>
{
    private readonly IPersonPhotosService _personPhotosService;

    public GetPersonPhotoQueryHandler(IPersonPhotosService personPhotosService)
    {
        _personPhotosService = personPhotosService ?? throw new ArgumentNullException(nameof(personPhotosService));
    }

    public async Task<GetPersonPhotoResult> Handle(GetPersonPhotoQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var photo = await _personPhotosService.GetPersonPhoto(query.PersonId, token);

        return new GetPersonPhotoResult(photo);
    }
}