using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.RepeatMeeting;

public sealed record RepeatMeetingCommand(MessageContext MessageContext)
    : IDialogCommand;