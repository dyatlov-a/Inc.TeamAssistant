using Inc.TeamAssistant.CheckIn.Client;
using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.TelegramConnector;
using Inc.TeamAssistant.TelegramConnector.Internal;
using Inc.TeamAssistant.TelegramConnector.Internal.CheckIn;

var builder = WebApplication.CreateBuilder(args);

var checkInOptions = builder.Configuration.GetRequiredSection(nameof(CheckInBotOptions)).Get<CheckInBotOptions>()!;

builder.Services
    .AddHttpClient<ICheckInService, CheckInService>()
    .ConfigureHttpClient(o => o.BaseAddress = new Uri(checkInOptions.BaseUrl));

builder.Services
    .AddSingleton(sp => ActivatorUtilities.CreateInstance<CheckInBotMessageHandler>(
        sp,
        checkInOptions.ConnectToMapLinkTemplate))
    .AddHostedService<TelegramBotConnector>(sp => ActivatorUtilities.CreateInstance<TelegramBotConnector>(
        sp,
        sp.GetRequiredService<CheckInBotMessageHandler>(),
        checkInOptions.AccessToken));
    
var app = builder.Build();

app.Run();