using Inc.TeamAssistant.Connector.Model.Commands.ChangeTeamProperty;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Handlers;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class ChangeTeamPropertyCommandFactory : IChangeTeamPropertyCommandFactory
{
    public IDialogCommand Create(MessageContext messageContext, string propertyName, string propertyValue)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyValue);

        return new ChangeTeamPropertyCommand(
            messageContext,
            messageContext.TryParseId(),
            propertyName,
            propertyValue);
    }
}