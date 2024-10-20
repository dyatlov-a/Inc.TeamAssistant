namespace Inc.TeamAssistant.Gateway.Middlewares;

internal static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        
        app.UseMiddleware<RequestCultureMiddleware>();

        return app;
    }
}