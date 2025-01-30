using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetPersonPhoto;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetPersonPhoto;

internal sealed class GetPersonPhotoQueryHandler : IRequestHandler<GetPersonPhotoQuery, GetPersonPhotoResult>
{
    private readonly IPersonPhotoService _personPhotoService;

    public GetPersonPhotoQueryHandler(IPersonPhotoService personPhotoService)
    {
        _personPhotoService = personPhotoService ?? throw new ArgumentNullException(nameof(personPhotoService));
    }

    public async Task<GetPersonPhotoResult> Handle(GetPersonPhotoQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var photo = await _personPhotoService.GetPersonPhoto(query.PersonId, token);

        return new GetPersonPhotoResult(photo);
    }
}