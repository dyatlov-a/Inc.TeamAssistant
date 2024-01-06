using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.CommandFactories;

public interface ICommandFactory
{
	IRequest<CommandResult>? TryCreate(CommandContext context);
}