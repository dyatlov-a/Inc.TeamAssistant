using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.QueryHandlers.GetWidgets.Converters;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetWidgets;

internal sealed class GetWidgetsQueryHandler : IRequestHandler<GetWidgetsQuery, GetWidgetsResult>
{
    private readonly IBotReader _reader;
    private readonly IPersonResolver _personResolver;
    private readonly IDashboardSettingsRepository _repository;

    public GetWidgetsQueryHandler(
        IBotReader reader,
        IPersonResolver personResolver,
        IDashboardSettingsRepository repository)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GetWidgetsResult> Handle(GetWidgetsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var person = _personResolver.GetCurrentPerson();
        var features = await _reader.GetFeatures(query.BotId, token);
        var dashboardSettings = await _repository.Find(person.Id, query.BotId, token);
        var widgets = DashboardSettingsConverter.Convert(
            dashboardSettings ?? DashboardSettings.CreateDefaultSettings(person.Id, query.BotId),
            features);

        return new GetWidgetsResult(widgets);
    }
}