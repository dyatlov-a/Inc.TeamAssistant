using Inc.TeamAssistant.Connector.Application.CommandHandlers.ChangeTeamProperty.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.End.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.Help.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeam.Services;
using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Connector.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectorApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<CommandFactory>()
            .AddSingleton<DialogCommandFactory>()
            .AddSingleton<MessageContextBuilder>()
            .AddSingleton<DialogContinuation>()
            .AddSingleton<TelegramBotMessageHandler>()
            .AddHostedService<TelegramBotConnector>()
            .AddSingleton<ICommandExecutor, CommandExecutor>()
            .AddSingleton<TelegramBotClientProvider>()
            .AddSingleton<AliasService>()
            .AddSingleton<SingleLineCommandFactory>()
            .AddSingleton<CommandCreatorResolver>()
            .AddSingleton<BotConstructor>()
            .AddSingleton<IBotAccessor, BotAccessor>()
            .AddSingleton<IBotConnector, BotConnector>()
            .AddSingleton<IBotListeners, BotListeners>()
            .AddHostedService<PhotosService>()
            
            .AddTransient(typeof(IRequestPostProcessor<,>), typeof(CommandPostProcessor<,>))
            
            .AddSingleton<ICommandCreator, CreateTeamCommandCreator>()
            .AddSingleton<ICommandCreator, EndCommandCreator>()
            .AddSingleton<ICommandCreator, HelpCommandCreator>()
            .AddSingleton<ICommandCreator, JoinToTeamCommandCreator>()
            .AddSingleton<ICommandCreator, LeaveFromTeamCommandCreator>()
            .AddSingleton<ICommandCreator, RemoveTeamCommandCreator>()
            
            .AddSingleton<ICommandCreator>(sp => ActivatorUtilities.CreateInstance<ChangeTeamPropertyCommandCreator>(
                sp,
                CommandList.MoveToSp,
                "storyType",
                "Scrum"))
            .AddSingleton<ICommandCreator>(sp => ActivatorUtilities.CreateInstance<ChangeTeamPropertyCommandCreator>(
                sp,
                CommandList.MoveToTShirts,
                "storyType",
                "Kanban"))
            
            .AddSingleton<ICommandCreator>(sp => ActivatorUtilities.CreateInstance<ChangeTeamPropertyCommandCreator>(
                sp,
                CommandList.ChangeToRoundRobin,
                "nextReviewerStrategy",
                "RoundRobin"))
            .AddSingleton<ICommandCreator>(sp => ActivatorUtilities.CreateInstance<ChangeTeamPropertyCommandCreator>(
                sp,
                CommandList.ChangeToRandom,
                "nextReviewerStrategy",
                "Random"));
        
        return services;
    }
}