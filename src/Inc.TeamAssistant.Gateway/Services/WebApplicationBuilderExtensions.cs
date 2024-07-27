using Inc.TeamAssistant.Gateway.ExceptionHandlers;
using Serilog;
using Serilog.Events;
using Prometheus.DotNetRuntime;

namespace Inc.TeamAssistant.Gateway.Services;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder UseTelemetry(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var analyticsOptions = builder.Configuration
            .GetSection(nameof(AnalyticsOptions))
            .Get<AnalyticsOptions>() ?? new AnalyticsOptions();
        
        DotNetRuntimeStatsBuilder
            .Customize()
            .WithGcStats()
            .WithThreadPoolStats()
            .StartCollecting();

        builder.Services
            .AddSingleton(analyticsOptions)
            .AddProblemDetails()
            .AddExceptionHandler<UnhandledExceptionHandler>();
        
        builder.Host.UseSerilog((_, c) =>
        {
            c.Enrich.FromLogContext();
            c.MinimumLevel.Information();

            if (string.IsNullOrWhiteSpace(analyticsOptions.SentryDsn))
                c.WriteTo.Console();
            else
                c.WriteTo.Sentry(s =>
                {
                    s.Environment = builder.Environment.EnvironmentName;
                    s.Dsn = analyticsOptions.SentryDsn;
                    s.MinimumBreadcrumbLevel = LogEventLevel.Information;
                    s.MinimumEventLevel = LogEventLevel.Error;
                });
        });
        
        return builder;
    }
}