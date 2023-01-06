using MediatR;

namespace Inc.TeamAssistant.Appraiser.Backend.Services.CommandFactories;

public interface ICommandFactory
{
	IBaseRequest? TryCreate(CommandContext context);
}