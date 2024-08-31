using System.Text.Encodings.Web;
using System.Text.Unicode;
using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.DataAccess;
using Inc.TeamAssistant.WebUI.Services;
using Inc.TeamAssistant.CheckIn.Application;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.DataAccess;
using Inc.TeamAssistant.CheckIn.Geo;
using Inc.TeamAssistant.Connector.Application;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.DataAccess;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.DataAccess;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Gateway.Hubs;
using Inc.TeamAssistant.Gateway.Services;
using Inc.TeamAssistant.Reviewer.Application;
using Inc.TeamAssistant.Reviewer.DataAccess;
using Prometheus;
using Inc.TeamAssistant.Gateway.Components;
using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Gateway.Services.ServerCore;
using Inc.TeamAssistant.Holidays.Model;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.RandomCoffee.Application;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.DataAccess;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.WebEncoders;
using Serilog;

ValidatorOptions.Global.Configure(LanguageSettings.DefaultLanguageId);

var builder = WebApplication
	.CreateBuilder(args)
	.ConfigureTelemetry()
	.ConfigureDataProtection();

var defaultLifetime = ServiceLifetime.Scoped;
var connectionString = builder.Configuration.GetConnectionString("ConnectionString")!;
var linksOptions = builder.Configuration.GetRequiredSection(nameof(LinksOptions)).Get<LinksOptions>()!;
var authOptions = builder.Configuration.GetRequiredSection(nameof(AuthOptions)).Get<AuthOptions>()!;
var openGraphOptions = builder.Configuration.GetRequiredSection(nameof(OpenGraphOptions)).Get<OpenGraphOptions>()!;

builder.Services
	.AddValidatorsFromAssemblyContaining<IStoryRepository>(defaultLifetime, includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ILocationsRepository>(defaultLifetime, includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ITaskForReviewRepository>(defaultLifetime, includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ITeamRepository>(defaultLifetime, includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<IRandomCoffeeRepository>(defaultLifetime, includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<IBotRepository>(defaultLifetime, includeInternalTypes: true)
	.AddMediatR(c =>
	{
		c.Lifetime = defaultLifetime;
		c.RegisterServicesFromAssemblyContaining<IStoryRepository>();
		c.RegisterServicesFromAssemblyContaining<ILocationsRepository>();
		c.RegisterServicesFromAssemblyContaining<ITaskForReviewRepository>();
		c.RegisterServicesFromAssemblyContaining<ITeamRepository>();
		c.RegisterServicesFromAssemblyContaining<IRandomCoffeeRepository>();
		c.RegisterServicesFromAssemblyContaining<IBotRepository>();
	})
	.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>))
	.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>)));

builder.Services
	.AddDataAccess(connectionString)
	.AddMessageIdType()
	.AddLanguageIdType()
	.AddDateOnlyType()
	.AddDateTimeOffsetType()
	.AddJsonType<ICollection<string>>()
	.AddJsonType<ICollection<long>>()
	.AddJsonType<ICollection<PersonPair>>()
	.AddJsonType<IReadOnlyCollection<string>>()
	.AddJsonType<IReadOnlyCollection<ReviewInterval>>()
	.AddJsonType<IReadOnlyCollection<ContextScope>>()
	.AddJsonType<IReadOnlyCollection<DayOfWeek>>()
	.AddJsonType<IReadOnlyDictionary<DateOnly, HolidayType>>()
	.AddJsonType<IReadOnlyDictionary<string, string>>()
	.AddJsonType<WorkScheduleUtc>()
	.Build();

builder.Services
	.AddHolidays(CachePolicies.CacheAbsoluteExpiration)
	.AddServices(authOptions, openGraphOptions, builder.Environment.WebRootPath, CachePolicies.CacheAbsoluteExpiration)
	.AddIsomorphic()
	.AddAppraiserApplication(linksOptions.ConnectToDashboardLinkTemplate)
	.AddAppraiserDataAccess()
	.AddCheckInApplication(linksOptions.ConnectToMapLinkTemplate)
	.AddCheckInDataAccess()
	.AddCheckInGeoCountry(builder.Environment.WebRootPath)
	.AddReviewerApplication()
	.AddReviewerDataAccess()
	.AddRandomCoffeeApplication()
	.AddRandomCoffeeDataAccess()
	.AddConnectorApplication(CachePolicies.UserAvatarCacheDurationInSeconds)
	.AddConnectorDataAccess(CachePolicies.CacheAbsoluteExpiration)
	.AddConstructorDataAccess();

builder.Services
	.AddAuthentication(ApplicationContext.AuthenticationScheme)
	.AddCookie(o =>
	{
		o.ExpireTimeSpan = TimeSpan.FromDays(2);
		o.SlidingExpiration = true;
		o.AccessDeniedPath = "/";
		o.LoginPath = "/login";
	});

builder.Services
	.AddHttpContextAccessor()
	.AddMemoryCache()
	.AddOutputCache(c => c.AddPolicy(
		CachePolicies.OpenGraphCachePolicyName,
		b => b.Expire(TimeSpan.FromSeconds(CachePolicies.OpenGraphCacheDurationInSeconds))))
	.Configure<WebEncoderOptions>(c => c.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All))
	.AddMvc();

builder.Services
	.AddHealthChecks();

builder.Services
	.AddSignalR();

builder.Services
	.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
	app.UseWebAssemblyDebugging();

app
	.UseSerilogRequestLogging()
	.UseStatusCodePagesWithReExecute("/error404")
	.UseExceptionHandler()
	.UseStaticFiles()
	.UseRouting()
	.UseOutputCache()
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