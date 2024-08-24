using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Primitives.Handlers;

public interface IChangeTeamPropertyCommandFactory
{
    IDialogCommand Create(MessageContext messageContext, string propertyName, string propertyValue);
}