using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.UpdateWidgets;

public sealed record UpdateWidgetsCommand(
    Guid BotId,
    IReadOnlyDictionary<string, UpdateWidgetDto> Widgets)
    : IRequest;