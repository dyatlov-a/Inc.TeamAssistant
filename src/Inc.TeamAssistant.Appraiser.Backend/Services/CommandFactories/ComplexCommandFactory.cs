using MediatR;

namespace Inc.TeamAssistant.Appraiser.Backend.Services.CommandFactories;

internal sealed class ComplexCommandFactory : ICommandFactory
{
	private readonly StaticCommandFactory _staticCommandFactory;
	private readonly DynamicCommandFactory _dynamicCommandFactory;

    public ComplexCommandFactory(StaticCommandFactory staticCommandFactory, DynamicCommandFactory dynamicCommandFactory)
	{
		_staticCommandFactory = staticCommandFactory ?? throw new ArgumentNullException(nameof(staticCommandFactory));
		_dynamicCommandFactory = dynamicCommandFactory ?? throw new ArgumentNullException(nameof(dynamicCommandFactory));
    }

	public IBaseRequest? TryCreate(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var command = _staticCommandFactory.TryCreate(context);

		command ??= _dynamicCommandFactory.TryCreate(context);

		return command;
	}
}