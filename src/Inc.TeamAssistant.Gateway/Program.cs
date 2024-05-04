using System.Text.Encodings.Web;
using System.Text.Unicode;
using Inc.TeamAssistant.Appraiser.Application;
using Inc.TeamAssistant.Appraiser.DataAccess;
using Inc.TeamAssistant.WebUI.Services;
using Inc.TeamAssistant.CheckIn.Application;
using Inc.TeamAssistant.CheckIn.DataAccess;
using Inc.TeamAssistant.Connector.Application;
using Inc.TeamAssistant.Connector.DataAccess;
using Inc.TeamAssistant.Gateway;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Gateway.Hubs;
using Inc.TeamAssistant.Gateway.Services;
using Inc.TeamAssistant.Reviewer.Application;
using Inc.TeamAssistant.Reviewer.DataAccess;
using Prometheus;
using Inc.TeamAssistant.Gateway.Components;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.RandomCoffee.Application;
using Inc.TeamAssistant.RandomCoffee.DataAccess;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.WebEncoders;

var builder = WebApplication.CreateBuilder(args);

var cacheAbsoluteExpiration = builder.Configuration.GetRequiredSection("CacheAbsoluteExpiration").Get<TimeSpan>();
var appraiserOptions = builder.Configuration.GetRequiredSection(nameof(AppraiserOptions)).Get<AppraiserOptions>()!;
var connectionString = builder.Configuration.GetConnectionString("ConnectionString")!;
var checkInOptions = builder.Configuration.GetRequiredSection(nameof(CheckInOptions)).Get<CheckInOptions>()!;
var reviewerOptions = builder.Configuration.GetRequiredSection(nameof(ReviewerOptions)).Get<ReviewerOptions>()!;
var workdayOptions = builder.Configuration.GetRequiredSection(nameof(WorkdayOptions)).Get<WorkdayOptions>()!;
var randomCoffeeOptions = builder.Configuration.GetRequiredSection(nameof(RandomCoffeeOptions)).Get<RandomCoffeeOptions>()!;
var accountsOptions = builder.Configuration.GetRequiredSection(nameof(AuthOptions)).Get<AuthOptions>()!;

builder.Services
	.AddSingleton(accountsOptions)
	.AddScoped<AuthService>()
	.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(o =>
	{
		o.ExpireTimeSpan = TimeSpan.FromDays(2);
		o.SlidingExpiration = true;
		o.AccessDeniedPath = "/";
	});

builder.Services
	.AddDataAccess(connectionString)
	.AddMessageIdType()
	.AddLanguageIdType()
	.AddDateOnlyType()
	.AddDateTimeOffsetType()
	.AddJsonType<ICollection<string>>()
	.AddJsonType<ICollection<long>>()
	.AddJsonType<ICollection<PersonPair>>()
	.AddJsonType<IReadOnlyDictionary<string, string>>()
	.Build()
		
	.AddValidators(LanguageSettings.DefaultLanguageId)
	.AddHandlers()
	.AddHolidays(workdayOptions, cacheAbsoluteExpiration)
	.AddServices(builder.Environment.WebRootPath, cacheAbsoluteExpiration)
	.AddIsomorphic()
		
	.AddAppraiserApplication(appraiserOptions)
	.AddAppraiserDataAccess()
	.AddCheckInApplication(checkInOptions)
	.AddCheckInDataAccess()
	.AddReviewerApplication(reviewerOptions)
	.AddReviewerDataAccess()
	.AddRandomCoffeeApplication(randomCoffeeOptions)
	.AddRandomCoffeeDataAccess()
	.AddConnectorApplication()
	.AddConnectorDataAccess(cacheAbsoluteExpiration)
	
	.AddMemoryCache()
	.AddHttpContextAccessor()
	.AddTelemetry()
	.Configure<WebEncoderOptions>(c => c.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All))
	.AddMvc();

builder.Services
	.AddSignalR();

builder.Services
	.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
	app.UseWebAssemblyDebugging();

app
	.UseStaticFiles()
	.UseRouting()
	.UseAuthentication()
	.UseAuthorization()
	.UseAntiforgery()
	.UseEndpoints(e =>
	{
		e.MapDefaultControllerRoute();
        e.MapMetrics();
        e.MapHealthChecks("/health");
	});

app
	.MapRazorComponents<App>()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(IRenderContext).Assembly);

app.MapHub<MessagesHub>("/messages");

app.Run();