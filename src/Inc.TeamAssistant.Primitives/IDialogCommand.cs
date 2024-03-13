using MediatR;

namespace Inc.TeamAssistant.Primitives;

public interface IDialogCommand : IRequest<CommandResult>
{
    MessageContext MessageContext { get; }
}