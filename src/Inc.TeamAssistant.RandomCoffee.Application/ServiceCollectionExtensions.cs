using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.AddPollAnswer.Services;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee.Services;
using Inc.TeamAssistant.RandomCoffee.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.RandomCoffee.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRandomCoffeeApplication(
        this IServiceCollection services,
        RandomCoffeeOptions options)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        services
            .AddSingleton(options)
            
            .AddSingleton<ICommandCreator, InviteForCoffeeCommandCreator>()
            .AddSingleton<ICommandCreator, AddPollAnswerCommandCreator>()
            
            .AddHostedService<ScheduleService>();

        return services;
    }
}