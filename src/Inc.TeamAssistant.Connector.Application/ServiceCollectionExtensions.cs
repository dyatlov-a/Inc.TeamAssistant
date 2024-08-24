using Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.End.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.Help.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeam.Services;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Handlers;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Connector.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectorApplication(
        this IServiceCollection services,
        int userAvatarCacheDurationInSeconds)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<CommandFactory>()
            .AddSingleton<DialogCommandFactory>()
            .AddSingleton<MessageContextBuilder>()
            .AddSingleton<DialogContinuation>()
            .AddSingleton<UpdateHandlerFactory>()
            .AddHostedService<TelegramBotConnector>()
            .AddSingleton<ICommandExecutor, CommandExecutor>()
            .AddSingleton<TelegramBotClientProvider>()
            .AddSingleton<AliasService>()
            .AddSingleton<SingleLineCommandFactory>()
            .AddSingleton<CommandCreatorResolver>()
            .AddSingleton<IBotAccessor, BotAccessor>()
            .AddSingleton<ContextCommandConverter>()
            .AddSingleton<IBotConnector, BotConnector>()
            .AddSingleton<IBotListeners, BotListeners>()

            .AddSingleton<PersonPhotosService>()
            .AddSingleton<IPersonPhotosService>(sp => ActivatorUtilities.CreateInstance<PersonPhotosServiceCache>(
                sp,
                sp.GetRequiredService<PersonPhotosService>(),
                userAvatarCacheDurationInSeconds))

            .AddTransient(typeof(IRequestPostProcessor<,>), typeof(CommandPostProcessor<,>))

            .AddSingleton<IChangeTeamPropertyCommandFactory, ChangeTeamPropertyCommandFactory>()
            .AddSingleton<ICommandCreator, CreateTeamCommandCreator>()
            .AddSingleton<ICommandCreator, EndCommandCreator>()
            .AddSingleton<ICommandCreator, HelpCommandCreator>()
            .AddSingleton<ICommandCreator, JoinToTeamCommandCreator>()
            .AddSingleton<ICommandCreator, LeaveFromTeamCommandCreator>()
            .AddSingleton<ICommandCreator, RemoveTeamCommandCreator>();
        
        return services;
    }
}