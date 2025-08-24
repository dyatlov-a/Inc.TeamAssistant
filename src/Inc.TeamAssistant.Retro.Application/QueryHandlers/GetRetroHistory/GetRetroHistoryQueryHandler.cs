using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroHistory;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroHistory;

internal sealed class GetRetroHistoryQueryHandler : IRequestHandler<GetRetroHistoryQuery, GetRetroHistoryResult>
{
    private readonly IRetroSessionReader _retroSessionReader;
    private readonly IRetroTemplateReader _retroTemplateReader;
    private readonly IRetroSessionRepository _repository;
    private readonly IRetroAssessmentReader _assessmentReader;

    public GetRetroHistoryQueryHandler(
        IRetroSessionReader retroSessionReader,
        IRetroTemplateReader retroTemplateReader,
        IRetroSessionRepository repository,
        IRetroAssessmentReader assessmentReader)
    {
        _retroSessionReader = retroSessionReader ?? throw new ArgumentNullException(nameof(retroSessionReader));
        _retroTemplateReader = retroTemplateReader ?? throw new ArgumentNullException(nameof(retroTemplateReader));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _assessmentReader = assessmentReader ?? throw new ArgumentNullException(nameof(assessmentReader));
    }

    public async Task<GetRetroHistoryResult> Handle(GetRetroHistoryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var session = await query.RetroSessionId.Required(_repository.Find, token);
        if (session.State != RetroSessionState.Finished)
            return GetRetroHistoryResult.Empty;

        var items = await _retroSessionReader.ReadItems(session.Id, token);
        var actions = await _retroSessionReader.ReadActionItems(session.Id, token);
        var columns = await _retroTemplateReader.GetColumns(session.TemplateId, token);
        var assessments = await _assessmentReader.Read(session.Id, token);

        var retroItems = items.Select(RetroItemConverter.ConvertFromHistory).ToArray();
        var actionItems = actions.Select(ActionItemConverter.ConvertTo).ToArray();
        var retroColumns = columns.Select(RetroColumnConverter.ConvertTo).ToArray();

        return new GetRetroHistoryResult(
            RetroSessionConverter.ConvertTo(session),
            retroItems,
            actionItems,
            retroColumns,
            assessments);
    }
}