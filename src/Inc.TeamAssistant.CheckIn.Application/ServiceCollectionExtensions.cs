using Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.CheckIn.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckInApplication(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddMediatR(c => c.AsScoped(), typeof(AddLocationToMapCommandHandler));

        return services;
    }
}