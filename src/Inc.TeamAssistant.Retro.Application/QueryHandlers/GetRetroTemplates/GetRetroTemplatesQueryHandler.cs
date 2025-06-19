using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroTemplates;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroTemplates;

internal sealed class GetRetroTemplatesQueryHandler : IRequestHandler<GetRetroTemplatesQuery, GetRetroTemplatesResult>
{
    private readonly IRetroTemplateReader _reader;

    public GetRetroTemplatesQueryHandler(IRetroTemplateReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetRetroTemplatesResult> Handle(GetRetroTemplatesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var templates = await _reader.GetTemplates(token);
        var items = templates
            .Select(t => new RetroTemplateDto(t.Id, t.Name))
            .OrderBy(t => t.Name)
            .ToArray();

        return new GetRetroTemplatesResult(items);
    }
}