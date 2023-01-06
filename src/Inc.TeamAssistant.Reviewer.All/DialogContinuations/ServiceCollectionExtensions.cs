using Inc.TeamAssistant.Reviewer.All.DialogContinuations.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Reviewer.All.DialogContinuations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDialogContinuations(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<IDialogContinuation, DialogContinuation>();

        return services;
    }
}