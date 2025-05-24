using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Hubs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventSenders(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<IAssessmentSessionEventSender, AssessmentSessionEventSender>()
            .AddScoped<IRetroEventSender, RetroEventSender>();
        
        return services;
    }
}