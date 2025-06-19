using Inc.TeamAssistant.Retro.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Retro.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRetroDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<IRetroItemRepository, RetroItemRepository>()
            .AddSingleton<IRetroSessionRepository, RetroSessionRepository>()
            .AddSingleton<IActionItemRepository, ActionItemRepository>()
            .AddSingleton<IRetroAssessmentRepository, RetroAssessmentRepository>()
            
            .AddSingleton<IRetroReader, RetroReader>()
            .AddSingleton<IActionItemReader, ActionItemReader>()
            .AddSingleton<IRetroTemplateReader, RetroTemplateReader>()
            .AddSingleton<IRetroAssessmentReader, RetroAssessmentReader>();

        return services;
    }
}