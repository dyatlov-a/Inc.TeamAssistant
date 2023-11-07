using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.CommandFactories;

public interface ICommandFactory
{
	IRequest<CommandResult>? TryCreate(CommandContext context);
}