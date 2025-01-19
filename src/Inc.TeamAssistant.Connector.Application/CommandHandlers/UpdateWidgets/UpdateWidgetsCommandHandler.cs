using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.UpdateWidgets;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.UpdateWidgets;

internal sealed class UpdateWidgetsCommandHandler : IRequestHandler<UpdateWidgetsCommand>
{
    private readonly ICurrentPersonResolver _personResolver;
    private readonly IDashboardSettingsRepository _repository;

    public UpdateWidgetsCommandHandler(ICurrentPersonResolver personResolver, IDashboardSettingsRepository repository)
    {
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task Handle(UpdateWidgetsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var person = _personResolver.GetCurrentPerson();
        var dashboardSettings = await _repository.Find(person.Id, command.BotId, token)
                                ?? DashboardSettings.CreateDefaultSettings(person.Id, command.BotId);

        foreach (var widget in command.Widgets)
            dashboardSettings.ChangeWidget(widget.Key, widget.Value.Position, widget.Value.IsEnabled);

        await _repository.Upsert(dashboardSettings, token);
    }
}