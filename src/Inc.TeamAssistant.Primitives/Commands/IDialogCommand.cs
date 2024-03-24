using MediatR;

namespace Inc.TeamAssistant.Primitives.Commands;

public interface IDialogCommand : IRequest<CommandResult>
{
    MessageContext MessageContext { get; }
}