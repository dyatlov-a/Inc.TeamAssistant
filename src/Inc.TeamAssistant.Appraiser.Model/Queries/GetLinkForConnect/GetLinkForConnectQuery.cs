using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetLinkForConnect;

public sealed record GetLinkForConnectQuery : IRequest<GetLinkForConnectResult>;