using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Primitives.Features.Teams;

public interface IChangeTeamPropertyCommandFactory
{
    IDialogCommand Create(MessageContext messageContext, string propertyName, string propertyValue);
}