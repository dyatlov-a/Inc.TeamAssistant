using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;

public sealed record GetWidgetsQuery(Guid BotId)
    : IRequest<GetWidgetsResult>;