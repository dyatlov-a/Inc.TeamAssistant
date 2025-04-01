using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.Model.Commands.SendMessage;

public sealed record SendMessageCommand(
    MessageContext MessageContext,
    MessageId MessageId,
    params object[] Values)
    : IDialogCommand;