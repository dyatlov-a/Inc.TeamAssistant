using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.CommandFactories;

public interface ICommandFactory
{
	IBaseRequest? TryCreate(CommandContext context);
}