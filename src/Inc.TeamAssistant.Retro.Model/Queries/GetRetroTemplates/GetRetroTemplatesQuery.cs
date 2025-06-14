using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroTemplates;

public sealed record GetRetroTemplatesQuery
    : IRequest<GetRetroTemplatesResult>;