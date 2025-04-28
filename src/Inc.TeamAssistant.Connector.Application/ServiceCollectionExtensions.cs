using Inc.TeamAssistant.Connector.Application.Alias;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.End.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.Help.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam.Services;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeam.Services;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Executors;
using Inc.TeamAssistant.Connector.Application.Parsers;
using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Connector.Application.Telegram;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Features.Teams;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

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
            .AddSingleton<TelegramMessageContextFactory>()
            .AddSingleton<MessageParser>()
            .AddSingleton<DialogContinuation>()
            .AddSingleton<TelegramUpdateHandlerFactory>()
            .AddHostedService<BotsHostedService>()
            .AddSingleton<ICommandExecutor, CommandExecutor>()
            .AddSingleton<TelegramMessageSender>()
            .AddSingleton<TelegramBotClientProvider>()
            .AddSingleton(new AliasService(AliasFinder.Find()))
            .AddSingleton<SingleLineCommandFactory>()
            .AddSingleton<CommandCreatorFactory>()
            .AddSingleton<IBotAccessor, BotAccessor>()
            .AddSingleton<TelegramContextCommandConverter>()
            .AddSingleton<IBotConnector, TelegramBotConnector>()
            .AddSingleton<IBotListeners, TelegramBotListeners>()
            .AddSingleton(new ReceiverOptions
            {
                AllowedUpdates = [
                    UpdateType.Message,
                    UpdateType.CallbackQuery,
                    UpdateType.PollAnswer,
                    UpdateType.EditedMessage]
            })

            .AddSingleton<TelegramPhotoService>()
            .AddSingleton<IPersonPhotoService>(sp => ActivatorUtilities.CreateInstance<PersonPhotoServiceCache>(
                sp,
                sp.GetRequiredService<TelegramPhotoService>(),
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