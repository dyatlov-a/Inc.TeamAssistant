using Inc.TeamAssistant.Appraiser.Application;
using Inc.TeamAssistant.Appraiser.Backend;
using Inc.TeamAssistant.Appraiser.Backend.Hubs;
using Inc.TeamAssistant.Appraiser.Backend.Services;
using Inc.TeamAssistant.Appraiser.DataAccess.InMemory;
using Inc.TeamAssistant.Appraiser.DataAccess.Postgres;
using Inc.TeamAssistant.Appraiser.Notifications;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.All;
using Inc.TeamAssistant.Reviewer.All.DialogContinuations;
using Inc.TeamAssistant.Reviewer.All.Holidays;
using Inc.TeamAssistant.WebUI.Services;
using Prometheus;
using Prometheus.DotNetRuntime;

var builder = WebApplication.CreateBuilder(args);

var telegramBotOptions = builder.Configuration.GetRequiredSection(nameof(TelegramBotOptions)).Get<TelegramBotOptions>()!;
var connectionString = builder.Configuration.GetConnectionString("ConnectionString")!;
var reviewerOptions = builder.Configuration.GetRequiredSection(nameof(ReviewerOptions)).Get<ReviewerOptions>()!;
var holidayOptions = builder.Configuration.GetRequiredSection(nameof(HolidayOptions)).Get<HolidayOptions>()!;

builder.Services
	.AddHttpContextAccessor()
	.AddMemoryCache()
	
    .AddApplication(builder.Configuration)
    .AddInMemoryDataAccess()
    .AddPostgresDataAccess(connectionString, Settings.AnonymousUser)
	.AddNotifications(CommandList.SetEstimateForStory, CommandList.NoIdea)
	.AddServices(telegramBotOptions)

    .AddReviewer(reviewerOptions, connectionString)
	
    .AddHolidays(connectionString, holidayOptions)
    .AddDialogContinuations()

	.AddIsomorphic()
    .AddMvc();

builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}

app
	.UseStaticFiles()
	.UseBlazorFrameworkFiles()
	.UseRouting()
	.UseEndpoints(e =>
	{
		e.MapDefaultControllerRoute();
		e.MapFallbackToPage("/_Host");
        e.MapMetrics();
        e.MapHealthChecks("/health");
    });

app.MapHub<MessagesHub>("/messages");

DotNetRuntimeStatsBuilder
    .Customize()
    .WithGcStats()
    .WithThreadPoolStats()
    .StartCollecting();

app.Run();