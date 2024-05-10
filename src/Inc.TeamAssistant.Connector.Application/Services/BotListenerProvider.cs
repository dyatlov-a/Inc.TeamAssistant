using Inc.TeamAssistant.Primitives.Bots;
using Microsoft.Extensions.Hosting;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotListenerProvider : IBotListenerProvider
{
    private readonly IEnumerable<IHostedService> _hostedServices;

    public BotListenerProvider(IEnumerable<IHostedService> hostedServices)
    {
        _hostedServices = hostedServices ?? throw new ArgumentNullException(nameof(hostedServices));
    }

    public IBotListener Listener => _hostedServices.OfType<IBotListener>().Single();
}