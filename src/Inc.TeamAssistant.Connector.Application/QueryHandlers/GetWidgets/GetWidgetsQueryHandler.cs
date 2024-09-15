using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.QueryHandlers.GetWidgets.Converters;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetWidgets;

internal sealed class GetWidgetsQueryHandler : IRequestHandler<GetWidgetsQuery, GetWidgetsResult>
{
    private readonly IBotReader _botReader;
    private readonly ICurrentPersonResolver _personResolver;
    private readonly IDashboardSettingsRepository _repository;

    public GetWidgetsQueryHandler(
        IBotReader botReader,
        ICurrentPersonResolver personResolver,
        IDashboardSettingsRepository repository)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GetWidgetsResult> Handle(GetWidgetsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var person = _personResolver.GetCurrentPerson();
        var bots = await _botReader.GetBotsByUser(person.Id, token);
        var dashboardSettings = await _repository.Find(person.Id, query.BotId, token);
        
        var widgets = DashboardSettingsConverter.Convert(
            dashboardSettings?? DashboardSettings.CreateDefaultSettings(person.Id, query.BotId),
            bots.SingleOrDefault(b => b.Id == query.BotId)?.Features ?? Array.Empty<string>());

        return new GetWidgetsResult(widgets);
    }
}