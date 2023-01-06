using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetLinkForConnect;

public sealed record GetLinkForConnectQuery(int Width, int Height, bool DrawQuietZones)
    : IRequest<GetLinkForConnectResult>;