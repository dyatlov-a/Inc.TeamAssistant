using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetPersonPhoto;

public record GetPersonPhotoQuery(long PersonId)
    : IRequest<GetPersonPhotoResult>;