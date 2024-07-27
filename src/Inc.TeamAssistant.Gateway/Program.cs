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
using Inc.TeamAssistant.Connector.Application;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.DataAccess;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.DataAccess;
using Inc.TeamAssistant.Gateway;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Gateway.Hubs;
using Inc.TeamAssistant.Gateway.Services;
using Inc.TeamAssistant.Reviewer.Application;
using Inc.TeamAssistant.Reviewer.DataAccess;
using Prometheus;
using Inc.TeamAssistant.Gateway.Components;
using Inc.TeamAssistant.Gateway.Services.ServerCore;
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
	.UseTelemetry();

var connectionString = builder.Configuration.GetConnectionString("ConnectionString")!;
var cacheAbsoluteExpiration = builder.Configuration
	.GetRequiredSection("CacheAbsoluteExpiration")
	.Get<TimeSpan>();
var appraiserOptions = builder.Configuration
	.GetRequiredSection(nameof(AppraiserOptions))
	.Get<AppraiserOptions>()!;
var checkInOptions = builder.Configuration
	.GetRequiredSection(nameof(CheckInOptions))
	.Get<CheckInOptions>()!;
var reviewerOptions = builder.Configuration
	.GetRequiredSection(nameof(ReviewerOptions))
	.Get<ReviewerOptions>()!;
var workdayOptions = builder.Configuration
	.GetRequiredSection(nameof(WorkdayOptions))
	.Get<WorkdayOptions>()!;
var randomCoffeeOptions = builder.Configuration
	.GetRequiredSection(nameof(RandomCoffeeOptions))
	.Get<RandomCoffeeOptions>()!;
var authOptions = builder.Configuration
	.GetRequiredSection(nameof(AuthOptions))
	.Get<AuthOptions>()!;
var openGraphOptions = builder.Configuration
	.GetRequiredSection(nameof(OpenGraphOptions))
	.Get<OpenGraphOptions>()!;

builder.Services
	.AddValidatorsFromAssemblyContaining<IStoryRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ILocationsRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ITaskForReviewRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<ITeamRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<IRandomCoffeeRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true)
	.AddValidatorsFromAssemblyContaining<IBotRepository>(
		lifetime: ServiceLifetime.Scoped,
		includeInternalTypes: true);

builder.Services
	.AddMediatR(c =>
	{
		c.Lifetime = ServiceLifetime.Scoped;
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
	.AddAuthentication(ApplicationContext.AuthenticationScheme)
	.AddCookie(o =>
	{
		o.ExpireTimeSpan = TimeSpan.FromDays(2);
		o.SlidingExpiration = true;
		o.AccessDeniedPath = "/";
		o.LoginPath = "/login";
	});

builder.Services
	.AddDataAccess(connectionString)
	.AddMessageIdType()
	.AddLanguageIdType()
	.AddDateOnlyType()
	.AddDateTimeOffsetType()
	.AddJsonType<ICollection<string>>()
	.AddJsonType<IReadOnlyCollection<string>>()
	.AddJsonType<ICollection<long>>()
	.AddJsonType<ICollection<PersonPair>>()
	.AddJsonType<IReadOnlyDictionary<string, string>>()
	.AddJsonType<IReadOnlyCollection<ReviewInterval>>()
	.AddJsonType<ICollection<CommandScope>>()
	.Build()
	
	.AddSingleton(openGraphOptions)
	.AddHolidays(workdayOptions, cacheAbsoluteExpiration)
	.AddServices(authOptions, builder.Environment.WebRootPath, cacheAbsoluteExpiration)
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
	.AddConstructorDataAccess()
	
	.AddHttpContextAccessor()
	.AddMemoryCache()
	.AddOutputCache(c => c.AddPolicy(OutputCachePolicies.Images, b => b.Expire(TimeSpan.FromHours(1))))
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