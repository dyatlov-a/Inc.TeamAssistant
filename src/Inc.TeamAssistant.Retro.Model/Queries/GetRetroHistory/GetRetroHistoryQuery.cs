using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroHistory;

public sealed record GetRetroHistoryQuery(Guid RetroSessionId)
    : IRequest<GetRetroHistoryResult>;