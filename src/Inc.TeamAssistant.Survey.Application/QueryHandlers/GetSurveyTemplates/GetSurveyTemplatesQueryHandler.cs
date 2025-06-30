using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveyTemplates;

internal sealed class GetSurveyTemplatesQueryHandler
    : IRequestHandler<GetSurveyTemplatesQuery, GetSurveyTemplatesResult>
{
    private readonly ISurveyReader _reader;

    public GetSurveyTemplatesQueryHandler(ISurveyReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetSurveyTemplatesResult> Handle(GetSurveyTemplatesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var templates = await _reader.GetTemplates(token);
        var items = templates
            .Select(t => new SurveyTemplateDto(t.Id, t.Name))
            .OrderBy(t => t.Name)
            .ToArray();

        return new GetSurveyTemplatesResult(items);
    }
}